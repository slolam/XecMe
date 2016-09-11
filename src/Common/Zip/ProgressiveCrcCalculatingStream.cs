using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal class ProgressiveCrcCalculatingStream : Stream
    {
        // Fields
        private ZipIOBlockManager _blockManager;
        private Crc32Calculator _crcCalculator;
        private uint _expectedCrc;
        private long _highWaterMark;
        private Stream _underlyingStream;
        private bool _validateCrcWithExpectedCrc;

        // Methods
        internal ProgressiveCrcCalculatingStream(ZipIOBlockManager blockManager, Stream underlyingStream)
            : this(blockManager, underlyingStream, 0)
        {
            this._validateCrcWithExpectedCrc = false;
        }

        internal ProgressiveCrcCalculatingStream(ZipIOBlockManager blockManager, Stream underlyingStream, uint expectedCrc)
        {
            this._blockManager = blockManager;
            this._underlyingStream = underlyingStream;
            this._validateCrcWithExpectedCrc = true;
            this._expectedCrc = expectedCrc;
            this._highWaterMark = -1L;
        }

        internal uint CalculateCrc()
        {
            this.CheckDisposed();
            if (this._underlyingStream.CanSeek)
            {
                long position = this._underlyingStream.Position;
                if (this._highWaterMark == -1L)
                {
                    this.CrcCalculator.ClearCrc();
                    this._highWaterMark = 0L;
                }
                if (this._highWaterMark < this._underlyingStream.Length)
                {
                    this._underlyingStream.Position = this._highWaterMark;
                    this.CrcCalculator.CalculateStreamCrc(this._underlyingStream);
                    this._highWaterMark = this._underlyingStream.Length;
                }
            }
            return this.CrcCalculator.Crc;
        }

        private bool CanValidateCrcWithoutRead()
        {
            return (this._underlyingStream.CanSeek && (this._highWaterMark == this._underlyingStream.Length));
        }

        private void CheckDisposed()
        {
            if (this._underlyingStream == null)
            {
                throw new ObjectDisposedException(null, SR.Get("StreamObjectDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this._underlyingStream = null;
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
            this._underlyingStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            int num = 0;
            if (!this._underlyingStream.CanSeek)
            {
                num = this._underlyingStream.Read(buffer, offset, count);
                this.CrcCalculator.Accumulate(buffer, offset, num);
                return num;
            }
            long position = this._underlyingStream.Position;
            num = this._underlyingStream.Read(buffer, offset, count);
            if ((position == 0L) && (this._highWaterMark == -1L))
            {
                this._highWaterMark = 0L;
                this.CrcCalculator.ClearCrc();
            }
            if (position == this._highWaterMark)
            {
                this.CrcCalculator.Accumulate(buffer, offset, num);
                this._highWaterMark = this._underlyingStream.Position;
            }
            if ((this._validateCrcWithExpectedCrc && this.CanValidateCrcWithoutRead()) && (this.CrcCalculator.Crc != this._expectedCrc))
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            return num;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckDisposed();
            return this._underlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long newLength)
        {
            this.CheckDisposed();
            if (newLength < 0L)
            {
                throw new ArgumentOutOfRangeException("newLength");
            }
            if (newLength < this._highWaterMark)
            {
                this._highWaterMark = -1L;
            }
            this._underlyingStream.SetLength(newLength);
            this._validateCrcWithExpectedCrc = false;
            this._blockManager.DirtyFlag = true;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            if (!this._underlyingStream.CanSeek)
            {
                this._underlyingStream.Write(buffer, offset, count);
                this.CrcCalculator.Accumulate(buffer, offset, count);
            }
            else
            {
                long position = this._underlyingStream.Position;
                if (position < this._highWaterMark)
                {
                    this._highWaterMark = -1L;
                }
                this._underlyingStream.Write(buffer, offset, count);
                if (position == 0L)
                {
                    this._highWaterMark = 0L;
                    this.CrcCalculator.ClearCrc();
                }
                if (position == this._highWaterMark)
                {
                    this.CrcCalculator.Accumulate(buffer, offset, count);
                    this._highWaterMark = this._underlyingStream.Position;
                }
            }
            this._validateCrcWithExpectedCrc = false;
            this._blockManager.DirtyFlag = true;
        }

        // Properties
        public override bool CanRead
        {
            get
            {
                return ((this._underlyingStream != null) && this._underlyingStream.CanRead);
            }
        }

        public override bool CanSeek
        {
            get
            {
                return ((this._underlyingStream != null) && this._underlyingStream.CanSeek);
            }
        }

        public override bool CanWrite
        {
            get
            {
                return ((this._underlyingStream != null) && this._underlyingStream.CanWrite);
            }
        }

        private Crc32Calculator CrcCalculator
        {
            get
            {
                if (this._crcCalculator == null)
                {
                    this._crcCalculator = new Crc32Calculator();
                }
                return this._crcCalculator;
            }
        }

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                return this._underlyingStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this._underlyingStream.Position;
            }
            set
            {
                this.CheckDisposed();
                this._underlyingStream.Position = value;
            }
        }
    }
}
