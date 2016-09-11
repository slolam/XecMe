using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIOModeEnforcingStream : Stream, IDisposable
    {
        // Fields
        private FileAccess _access;
        private Stream _baseStream;
        private ZipIOLocalFileBlock _block;
        private ZipIOBlockManager _blockManager;
        private long _currentStreamPosition;
        private bool _disposedFlag;

        // Methods
        internal ZipIOModeEnforcingStream(Stream baseStream, FileAccess access, ZipIOBlockManager blockManager, ZipIOLocalFileBlock block)
        {
            this._baseStream = baseStream;
            this._access = access;
            this._blockManager = blockManager;
            this._block = block;
        }

        private void CheckDisposed()
        {
            if (this._disposedFlag)
            {
                throw new ObjectDisposedException(null, SR.Get("StreamObjectDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !this._disposedFlag)
                {
                    this._disposedFlag = true;
                    this._block.DeregisterExposedStream(this);
                    if ((this._access == FileAccess.ReadWrite) || (this._access == FileAccess.Write))
                    {
                        this._blockManager.SaveStream(this._block, true);
                    }
                }
            }
            finally
            {
                this._baseStream = null;
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.CheckDisposed();
            this._baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num;
            this.CheckDisposed();
            if (!this.CanRead)
            {
                throw new NotSupportedException(SR.Get("ReadNotSupported"));
            }
            long num2 = this._currentStreamPosition;
            try
            {
                this._baseStream.Seek(this._currentStreamPosition, SeekOrigin.Begin);
                num = this._baseStream.Read(buffer, offset, count);
                this._currentStreamPosition += num;
            }
            catch
            {
                this._currentStreamPosition = num2;
                throw;
            }
            return num;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckDisposed();
            long num = this._currentStreamPosition;
            if (origin == SeekOrigin.Begin)
            {
                num = offset;
            }
            else if (origin == SeekOrigin.Current)
            {
                num += offset;
            }
            else
            {
                if (origin != SeekOrigin.End)
                {
                    throw new ArgumentOutOfRangeException("origin");
                }
                num = this.Length + offset;
            }
            if (num < 0L)
            {
                throw new ArgumentException(SR.Get("SeekNegative"));
            }
            this._currentStreamPosition = num;
            return this._currentStreamPosition;
        }

        public override void SetLength(long newLength)
        {
            this.CheckDisposed();
            if (!this.CanWrite)
            {
                throw new NotSupportedException(SR.Get("SetLengthNotSupported"));
            }
            this._baseStream.SetLength(newLength);
            if (newLength < this._currentStreamPosition)
            {
                this._currentStreamPosition = newLength;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            if (!this.CanWrite)
            {
                throw new NotSupportedException(SR.Get("WriteNotSupported"));
            }
            if (this._baseStream.CanSeek)
            {
                this._baseStream.Seek(this._currentStreamPosition, SeekOrigin.Begin);
            }
            this._baseStream.Write(buffer, offset, count);
            this._currentStreamPosition += count;
        }

        // Properties
        public override bool CanRead
        {
            get
            {
                if (this._disposedFlag || !this._baseStream.CanRead)
                {
                    return false;
                }
                if (this._access != FileAccess.Read)
                {
                    return (this._access == FileAccess.ReadWrite);
                }
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return (!this._disposedFlag && this._baseStream.CanSeek);
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (this._disposedFlag || !this._baseStream.CanWrite)
                {
                    return false;
                }
                if (this._access != FileAccess.Write)
                {
                    return (this._access == FileAccess.ReadWrite);
                }
                return true;
            }
        }

        internal bool Disposed
        {
            get
            {
                return this._disposedFlag;
            }
        }

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                return this._baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this._currentStreamPosition;
            }
            set
            {
                this.CheckDisposed();
                this.Seek(value, SeekOrigin.Begin);
            }
        }
    }



}
