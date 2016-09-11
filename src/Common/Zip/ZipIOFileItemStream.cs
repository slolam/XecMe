using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIOFileItemStream : Stream
    {
        // Fields
        private ZipIOLocalFileBlock _block;
        private ZipIOBlockManager _blockManager;
        private SparseMemoryStream _cachePrefixStream;
        private long _currentStreamLength;
        private long _currentStreamPosition;
        private bool _dataChanged;
        private bool _dirtyFlag;
        private bool _disposedFlag;
        private const long _highWaterMark = 0xa00000L;
        private const long _lowWaterMark = 0x19000L;
        private long _offset;
        private long _persistedOffset;
        private long _persistedSize;
        private SparseMemoryStream _sparseMemoryStreamSuffix;

        // Methods
        internal ZipIOFileItemStream(ZipIOBlockManager blockManager, ZipIOLocalFileBlock block, long persistedOffset, long persistedSize)
        {
            this._persistedOffset = persistedOffset;
            this._offset = persistedOffset;
            this._persistedSize = persistedSize;
            this._blockManager = blockManager;
            this._block = block;
            this._currentStreamLength = persistedSize;
        }

        private void CheckDisposed()
        {
            if (this._disposedFlag)
            {
                throw new ObjectDisposedException(null, SR.Get("ZipFileItemDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !this._disposedFlag)
                {
                    if (this._sparseMemoryStreamSuffix != null)
                    {
                        this._sparseMemoryStreamSuffix.Close();
                    }
                    if (this._cachePrefixStream != null)
                    {
                        this._cachePrefixStream.Close();
                    }
                }
            }
            finally
            {
                this._sparseMemoryStreamSuffix = null;
                this._cachePrefixStream = null;
                this._disposedFlag = true;
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.CheckDisposed();
            this._blockManager.SaveStream(this._block, false);
        }

        internal void Move(long shiftSize)
        {
            this.CheckDisposed();
            if (shiftSize != 0L)
            {
                this._offset += shiftSize;
                this._dirtyFlag = true;
            }
        }

        internal PreSaveNotificationScanControlInstruction PreSaveNotification(long offset, long size)
        {
            return ZipIOBlockManager.CommonPreSaveNotificationHandler(this._blockManager.Stream, offset, size, this._persistedOffset, Math.Min(this._persistedSize, this._currentStreamLength), ref this._cachePrefixStream);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            PackagingUtilities.VerifyStreamReadArgs(this, buffer, offset, count);
            if (count == 0)
            {
                return 0;
            }
            if (this._currentStreamLength <= this._currentStreamPosition)
            {
                return 0;
            }
            int num2 = 0;
            int num4 = 0;
            long num5 = 0L;
            int num3 = 0;
            long num = this._currentStreamPosition;
            if (num < this._persistedSize)
            {
                num5 = Math.Min(this._currentStreamLength, this._persistedSize) - num;
                num4 = (int)Math.Min((long)count, num5);
                this._blockManager.Stream.Seek(this._persistedOffset + num, SeekOrigin.Begin);
                num2 = this._blockManager.Stream.Read(buffer, offset, num4);
                num += num2;
                count -= num2;
                offset += num2;
                if (num2 < num4)
                {
                    this._currentStreamPosition = num;
                    return num2;
                }
            }
            if ((this._sparseMemoryStreamSuffix != null) && ((num + count) > this._persistedSize))
            {
                this._sparseMemoryStreamSuffix.Seek(num - this._persistedSize, SeekOrigin.Begin);
                num3 = this._sparseMemoryStreamSuffix.Read(buffer, offset, count);
                num += num3;
            }
            int num6 = num2 + num3;
            this._currentStreamPosition = num;
            return num6;
        }

        internal void Save()
        {
            this.CheckDisposed();
            if (this._dirtyFlag)
            {
                long moveBlockSourceOffset = this._persistedOffset;
                long moveBlockSize = Math.Min(this._persistedSize, this._currentStreamLength);
                long moveBlockTargetOffset = this._offset;
                long num = 0L;
                if (this._cachePrefixStream != null)
                {
                    moveBlockSourceOffset += this._cachePrefixStream.Length;
                    moveBlockTargetOffset += this._cachePrefixStream.Length;
                    moveBlockSize -= this._cachePrefixStream.Length;
                }
                this._blockManager.MoveData(moveBlockSourceOffset, moveBlockTargetOffset, moveBlockSize);
                num += moveBlockSize;
                if (this._cachePrefixStream != null)
                {
                    this._blockManager.Stream.Seek(this._offset, SeekOrigin.Begin);
                    this._cachePrefixStream.WriteToStream(this._blockManager.Stream);
                    num += this._cachePrefixStream.Length;
                    this._cachePrefixStream.Close();
                    this._cachePrefixStream = null;
                }
                if (this._sparseMemoryStreamSuffix != null)
                {
                    if (this._blockManager.Stream.Position != (this._offset + this._persistedSize))
                    {
                        this._blockManager.Stream.Seek(this._offset + this._persistedSize, SeekOrigin.Begin);
                    }
                    this._sparseMemoryStreamSuffix.WriteToStream(this._blockManager.Stream);
                    num += this._sparseMemoryStreamSuffix.Length;
                    this._sparseMemoryStreamSuffix.Close();
                    this._sparseMemoryStreamSuffix = null;
                }
                this._blockManager.Stream.Flush();
                this._persistedOffset = this._offset;
                this._persistedSize = num;
                this._dirtyFlag = false;
                this._dataChanged = false;
            }
        }

        internal void SaveStreaming()
        {
            this.CheckDisposed();
            if (this._dirtyFlag)
            {
                if (this._sparseMemoryStreamSuffix != null)
                {
                    this._sparseMemoryStreamSuffix.WriteToStream(this._blockManager.Stream);
                    this._persistedSize += this._sparseMemoryStreamSuffix.Length;
                    this._sparseMemoryStreamSuffix.Close();
                    this._sparseMemoryStreamSuffix = null;
                }
                this._dirtyFlag = false;
                this._dataChanged = false;
            }
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
                num = this._currentStreamLength + offset;
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
            if (newLength < 0L)
            {
                throw new ArgumentOutOfRangeException("newLength");
            }
            if (this._currentStreamLength != newLength)
            {
                this._dirtyFlag = true;
                this._dataChanged = true;
                if (newLength <= this._persistedSize)
                {
                    if (this._sparseMemoryStreamSuffix != null)
                    {
                        this._sparseMemoryStreamSuffix.Close();
                        this._sparseMemoryStreamSuffix = null;
                    }
                }
                else
                {
                    if (this._sparseMemoryStreamSuffix == null)
                    {
                        this._sparseMemoryStreamSuffix = new SparseMemoryStream(0x19000L, 0xa00000L);
                    }
                    this._sparseMemoryStreamSuffix.SetLength(newLength - this._persistedSize);
                }
                this._currentStreamLength = newLength;
                if (this._currentStreamLength < this._currentStreamPosition)
                {
                    this.Seek(this._currentStreamLength, SeekOrigin.Begin);
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            PackagingUtilities.VerifyStreamWriteArgs(this, buffer, offset, count);
            if (count != 0)
            {
                int num2 = 0;
                this._dirtyFlag = true;
                this._dataChanged = true;
                long num = this._currentStreamPosition;
                if (num < this._persistedSize)
                {
                    this._blockManager.Stream.Seek(this._persistedOffset + num, SeekOrigin.Begin);
                    num2 = (int)Math.Min((long)count, this._persistedSize - num);
                    this._blockManager.Stream.Write(buffer, offset, num2);
                    num += num2;
                    count -= num2;
                    offset += num2;
                }
                if ((num + count) > this._persistedSize)
                {
                    if (this._sparseMemoryStreamSuffix == null)
                    {
                        this._sparseMemoryStreamSuffix = new SparseMemoryStream(0x19000L, 0xa00000L);
                    }
                    this._sparseMemoryStreamSuffix.Seek(num - this._persistedSize, SeekOrigin.Begin);
                    this._sparseMemoryStreamSuffix.Write(buffer, offset, count);
                    num += count;
                }
                this._currentStreamPosition = num;
                this._currentStreamLength = Math.Max(this._currentStreamLength, this._currentStreamPosition);
            }
        }

        // Properties
        public override bool CanRead
        {
            get
            {
                return (!this._disposedFlag && this._blockManager.Stream.CanRead);
            }
        }

        public override bool CanSeek
        {
            get
            {
                return (!this._disposedFlag && this._blockManager.Stream.CanSeek);
            }
        }

        public override bool CanWrite
        {
            get
            {
                return (!this._disposedFlag && this._blockManager.Stream.CanWrite);
            }
        }

        internal bool DataChanged
        {
            get
            {
                return this._dataChanged;
            }
        }

        internal bool DirtyFlag
        {
            get
            {
                return this._dirtyFlag;
            }
        }

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                return this._currentStreamLength;
            }
        }

        internal long Offset
        {
            get
            {
                return this._offset;
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
