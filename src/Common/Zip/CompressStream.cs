using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Axp.Fx.Common.Zip
{
    internal class CompressStream : Stream
    {
        // Fields
        private Stream _baseStream;
        private long _cachedLength;
        private Stream _current;
        private bool _dirtyForClosing;
        private bool _dirtyForFlushing;
        private static byte[] _emptyDeflateStreamConstant;
        private const long _highWaterMark = 0xa00000L;
        private bool _lengthVerified;
        private const long _lowWaterMark = 0x19000L;
        private Mode _mode;
        private long _position;
        private const long _readPassThroughModeSeekThreshold = 0x40L;

        // Methods
        static CompressStream()
        {
            byte[] buffer = new byte[2];
            buffer[0] = 3;
            _emptyDeflateStreamConstant = buffer;
        }

        internal CompressStream(Stream baseStream, long length)
            : this(baseStream, length, false)
        {
        }

        internal CompressStream(Stream baseStream, long length, bool creating)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }
            if (length < -1L)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            this._baseStream = baseStream;
            this._cachedLength = length;
            this._dirtyForFlushing = false;
            this._dirtyForClosing = creating;
        }

        private void ChangeMode(Mode newMode)
        {
            if (newMode != this._mode)
            {
                if (this._current != null)
                {
                    this._current.Close();
                    this._dirtyForClosing = false;
                    this._dirtyForFlushing = false;
                }
                this._mode = newMode;
                switch (newMode)
                {
                    case Mode.Start:
                        this._current = null;
                        this._baseStream.Position = 0L;
                        return;

                    case Mode.ReadPassThrough:
                    case Mode.WritePassThrough:
                        this._current = new DeflateStream(this._baseStream, (newMode == Mode.WritePassThrough) ? CompressionMode.Compress : CompressionMode.Decompress, true);
                        return;

                    case Mode.Emulation:
                        {
                            SparseMemoryStream tempStream = new SparseMemoryStream(0x19000L, 0xa00000L);
                            this._current = new CompressEmulationStream(this._baseStream, tempStream, this._position, new DeflateEmulationTransform());
                            this.UpdateUncompressedDataLength(this._current.Length);
                            return;
                        }
                    case Mode.Disposed:
                        return;
                }
            }
        }

        private void CheckDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(null, SR.Get("StreamObjectDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this._mode != Mode.Disposed))
                {
                    this.Flush();
                    if (this._current != null)
                    {
                        this._current.Close();
                        this._current = null;
                    }
                    if (this._dirtyForClosing && ((this._baseStream.CanSeek && (this._baseStream.Length == 0L)) || (this._cachedLength <= 0L)))
                    {
                        this._baseStream.Write(_emptyDeflateStreamConstant, 0, 2);
                        this._baseStream.Flush();
                    }
                    this._baseStream = null;
                    this.ChangeMode(Mode.Disposed);
                    this._dirtyForClosing = false;
                    this._dirtyForFlushing = false;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.CheckDisposed();
            if (this._current != null)
            {
                this._current.Flush();
                this._dirtyForFlushing = false;
                if ((this._mode == Mode.Emulation) && (this.Length != 0L))
                {
                    this._dirtyForClosing = false;
                }
            }
            this._baseStream.Flush();
        }

        private static bool IsDeflateStreamEmpty(Stream s)
        {
            bool flag = false;
            if (s.CanSeek && s.CanRead)
            {
                byte[] buffer = new byte[2];
                flag = (s.Read(buffer, 0, 2) == 0) || ((buffer[0] == _emptyDeflateStreamConstant[0]) && (buffer[1] == _emptyDeflateStreamConstant[1]));
                s.Position = 0L;
                return flag;
            }
            return true;
        }

        internal bool IsDirty(bool closingFlag)
        {
            if (!closingFlag)
            {
                return this._dirtyForFlushing;
            }
            return this._dirtyForClosing;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            PackagingUtilities.VerifyStreamReadArgs(this, buffer, offset, count);
            if (count == 0)
            {
                return 0;
            }
            switch (this._mode)
            {
                case Mode.Start:
                    if (this._position != 0L)
                    {
                        this.ChangeMode(Mode.Emulation);
                        break;
                    }
                    this.ChangeMode(Mode.ReadPassThrough);
                    break;

                case Mode.WritePassThrough:
                    if (this._position != 0L)
                    {
                        this.ChangeMode(Mode.Emulation);
                        break;
                    }
                    this.ChangeMode(Mode.ReadPassThrough);
                    break;
            }
            if (this._current == null)
            {
                return 0;
            }
            int num = this._current.Read(buffer, offset, count);
            if ((this._mode == Mode.ReadPassThrough) && (num == 0))
            {
                this.UpdateUncompressedDataLength(this._position);
                this.ChangeMode(Mode.Start);
            }
            this._position += num;
            return num;
        }

        private long ReadPassThroughModeSeek(long bytesToSeek)
        {
            byte[] buffer = new byte[Math.Min(0x1000L, bytesToSeek)];
            while (bytesToSeek > 0L)
            {
                long num = Math.Min(bytesToSeek, (long)buffer.Length);
                num = this._current.Read(buffer, 0, (int)num);
                if (num == 0L)
                {
                    return bytesToSeek;
                }
                bytesToSeek -= num;
            }
            return bytesToSeek;
        }

        internal void Reset()
        {
            this.CheckDisposed();
            this.ChangeMode(Mode.Start);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckDisposed();
            if (!this.CanSeek)
            {
                throw new NotSupportedException(SR.Get("SeekNotSupported"));
            }
            long num = -1L;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    num = offset;
                    break;

                case SeekOrigin.Current:
                    num = this._position + offset;
                    break;

                case SeekOrigin.End:
                    this.ChangeMode(Mode.Emulation);
                    num = this.Length + offset;
                    break;
            }
            if (num < 0L)
            {
                throw new ArgumentException(SR.Get("SeekNegative"));
            }
            long bytesToSeek = num - this._position;
            if (bytesToSeek != 0L)
            {
                if (((bytesToSeek > 0L) && (bytesToSeek < 0x40L)) && (this._mode == Mode.ReadPassThrough))
                {
                    long num3 = this.ReadPassThroughModeSeek(bytesToSeek);
                    if (num3 > 0L)
                    {
                        this.UpdateUncompressedDataLength(num - num3);
                        this.ChangeMode(Mode.Start);
                    }
                }
                else
                {
                    this.ChangeMode(Mode.Emulation);
                    this._current.Position = num;
                }
                this._position = num;
            }
            return this._position;
        }

        public override void SetLength(long newLength)
        {
            this.CheckDisposed();
            if (!this.CanSeek)
            {
                throw new NotSupportedException(SR.Get("SetLengthNotSupported"));
            }
            this._lengthVerified = true;
            switch (this._mode)
            {
                case Mode.Start:
                case Mode.ReadPassThrough:
                case Mode.WritePassThrough:
                    if (newLength != 0L)
                    {
                        this.ChangeMode(Mode.Emulation);
                        break;
                    }
                    this.ChangeMode(Mode.Start);
                    this._baseStream.SetLength(0L);
                    this.UpdateUncompressedDataLength(newLength);
                    break;
            }
            if (this._mode == Mode.Emulation)
            {
                this._current.SetLength(newLength);
            }
            if (newLength < this._position)
            {
                this.Seek(newLength, SeekOrigin.Begin);
            }
            this._dirtyForFlushing = true;
            this._dirtyForClosing = true;
        }

        private void UpdateUncompressedDataLength(long dataLength)
        {
            if ((this._cachedLength >= 0L) && !this._lengthVerified)
            {
                if (this._cachedLength != dataLength)
                {
                    throw new FileFormatException(SR.Get("CompressLengthMismatch"));
                }
                this._lengthVerified = true;
            }
            this._cachedLength = dataLength;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            PackagingUtilities.VerifyStreamWriteArgs(this, buffer, offset, count);
            if (count != 0)
            {
                switch (this._mode)
                {
                    case Mode.Start:
                        if ((this._position != 0L) || !IsDeflateStreamEmpty(this._baseStream))
                        {
                            this.ChangeMode(Mode.Emulation);
                            break;
                        }
                        this.ChangeMode(Mode.WritePassThrough);
                        break;

                    case Mode.ReadPassThrough:
                        this.ChangeMode(Mode.Emulation);
                        break;
                }
                this._current.Write(buffer, offset, count);
                this._position += count;
                if (this._mode == Mode.WritePassThrough)
                {
                    this.CachedLength = this._position;
                }
                this._dirtyForFlushing = true;
                this._dirtyForClosing = true;
            }
        }

        // Properties
        private long CachedLength
        {
            get
            {
                return this._cachedLength;
            }
            set
            {
                this._cachedLength = value;
            }
        }

        public override bool CanRead
        {
            get
            {
                return ((this._mode != Mode.Disposed) && this._baseStream.CanRead);
            }
        }

        public override bool CanSeek
        {
            get
            {
                return ((this._mode != Mode.Disposed) && this._baseStream.CanSeek);
            }
        }

        public override bool CanWrite
        {
            get
            {
                return ((this._mode != Mode.Disposed) && this._baseStream.CanWrite);
            }
        }

        internal bool IsDisposed
        {
            get
            {
                return (this._mode == Mode.Disposed);
            }
        }

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                switch (this._mode)
                {
                    case Mode.Start:
                    case Mode.ReadPassThrough:
                    case Mode.WritePassThrough:
                        if (this.CachedLength < 0L)
                        {
                            if ((this._position == 0L) && IsDeflateStreamEmpty(this._baseStream))
                            {
                                return 0L;
                            }
                            this.ChangeMode(Mode.Emulation);
                            break;
                        }
                        return this.CachedLength;
                }
                this.UpdateUncompressedDataLength(this._current.Length);
                return this._current.Length;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this._position;
            }
            set
            {
                this.CheckDisposed();
                this.Seek(value - this._position, SeekOrigin.Current);
            }
        }

        // Nested Types
        private enum Mode
        {
            Start,
            ReadPassThrough,
            WritePassThrough,
            Emulation,
            Disposed
        }
    }

}
