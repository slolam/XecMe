using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal class SparseMemoryStream : Stream
    {
        // Fields
        private bool _autoCloseSmallBlockGaps;
        private long _currentStreamLength;
        private long _currentStreamPosition;
        private bool _disposedFlag;
        private const int _fixBlockInMemoryOverhead = 100;
        private long _highWaterMark;
        private bool _isolatedStorageMode;
        private Stream _isolatedStorageStream;
        private string _isolatedStorageStreamFileName;
        private long _lowWaterMark;
        private List<MemoryStreamBlock> _memoryStreamList;
        private MemoryStreamBlock _searchBlock;
        private TrackingMemoryStreamFactory _trackingMemoryStreamFactory;

        // Methods
        internal SparseMemoryStream(long lowWaterMark, long highWaterMark)
            : this(lowWaterMark, highWaterMark, true)
        {
        }

        internal SparseMemoryStream(long lowWaterMark, long highWaterMark, bool autoCloseSmallBlockGaps)
        {
            this._trackingMemoryStreamFactory = new TrackingMemoryStreamFactory();
            Invariant.Assert((lowWaterMark >= 0L) && (highWaterMark >= 0L));
            Invariant.Assert(lowWaterMark < highWaterMark);
            Invariant.Assert(lowWaterMark <= 0x7fffffffL);
            this._memoryStreamList = new List<MemoryStreamBlock>(5);
            this._lowWaterMark = lowWaterMark;
            this._highWaterMark = highWaterMark;
            this._autoCloseSmallBlockGaps = autoCloseSmallBlockGaps;
        }

        private bool CanCollapseWithPreviousBlock(MemoryStreamBlock memStreamBlock, long offset, long length)
        {
            if (!this._autoCloseSmallBlockGaps || (memStreamBlock == null))
            {
                return false;
            }
            long num = offset - (memStreamBlock.Offset + memStreamBlock.Stream.Length);
            return ((num <= 100L) && (((num + length) + memStreamBlock.Stream.Length) <= 0x7fffffffL));
        }

        private void CheckDisposed()
        {
            if (this._disposedFlag)
            {
                throw new ObjectDisposedException(SR.Get("StreamObjectDisposed"));
            }
        }

        private MemoryStreamBlock ConstructMemoryStreamFromWriteRequest(byte[] buffer, long writeRequestOffset, int writeRequestSize, int bufferOffset)
        {
            MemoryStreamBlock block = new MemoryStreamBlock(this._trackingMemoryStreamFactory.Create(writeRequestSize), writeRequestOffset);
            block.Stream.Seek(0L, SeekOrigin.Begin);
            block.Stream.Write(buffer, bufferOffset, writeRequestSize);
            return block;
        }

        private void CopyMemoryBlocksToStream(Stream targetStream)
        {
            long currentPos = 0L;
            foreach (MemoryStreamBlock block in this._memoryStreamList)
            {
                currentPos = this.SkipWrite(targetStream, currentPos, block.Offset);
                targetStream.Write(block.Stream.GetBuffer(), 0, (int)block.Stream.Length);
                currentPos += block.Stream.Length;
            }
            if (currentPos < this._currentStreamLength)
            {
                currentPos = this.SkipWrite(targetStream, currentPos, this._currentStreamLength);
            }
            targetStream.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !this._disposedFlag)
                {
                    foreach (MemoryStreamBlock block in this._memoryStreamList)
                    {
                        block.Stream.Close();
                    }
                    if (this._isolatedStorageStream != null)
                    {
                        this._isolatedStorageStream.Close();
                    }
                }
            }
            finally
            {
                this._disposedFlag = true;
                this._isolatedStorageStream = null;
                this._memoryStreamList = null;
                base.Dispose(disposing);
            }
        }

        private void EnsureIsolatedStoreStream()
        {
            if (this._isolatedStorageStream == null)
            {
                this._isolatedStorageStream = PackagingUtilities.CreateUserScopedIsolatedStorageFileStreamWithRandomName(3, out this._isolatedStorageStreamFileName);
            }
        }

        public override void Flush()
        {
            this.CheckDisposed();
        }

        private MemoryStreamBlock GetSearchBlockForOffset(long offset)
        {
            if (this._searchBlock == null)
            {
                this._searchBlock = new MemoryStreamBlock(null, offset);
            }
            else
            {
                this._searchBlock.Offset = offset;
            }
            return this._searchBlock;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num3;
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
            int num2 = (int)Math.Min((long)count, this._currentStreamLength - this._currentStreamPosition);
            if (this._isolatedStorageMode)
            {
                this._isolatedStorageStream.Seek(this._currentStreamPosition, SeekOrigin.Begin);
                num3 = this._isolatedStorageStream.Read(buffer, offset, num2);
            }
            else
            {
                Array.Clear(buffer, offset, num2);
                int num = this._memoryStreamList.BinarySearch(this.GetSearchBlockForOffset(this._currentStreamPosition));
                if (num < 0)
                {
                    num = ~num;
                }
                while (num < this._memoryStreamList.Count)
                {
                    long num4;
                    long num5;
                    MemoryStreamBlock block = this._memoryStreamList[num];
                    PackagingUtilities.CalculateOverlap(block.Offset, (long)((int)block.Stream.Length), this._currentStreamPosition, (long)num2, out num5, out num4);
                    if (num4 <= 0L)
                    {
                        break;
                    }
                    Array.Copy(block.Stream.GetBuffer(), (int)(num5 - block.Offset), buffer, (int)((offset + num5) - this._currentStreamPosition), (int)num4);
                    num++;
                }
                num3 = num2;
            }
            this._currentStreamPosition += num3;
            return num3;
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
                if (this._isolatedStorageMode)
                {
                    this._isolatedStorageStream.SetLength(newLength);
                }
                else if (this._currentStreamLength > newLength)
                {
                    int index = this._memoryStreamList.BinarySearch(this.GetSearchBlockForOffset(newLength));
                    if (index < 0)
                    {
                        index = ~index;
                    }
                    else
                    {
                        MemoryStreamBlock block = this._memoryStreamList[index];
                        long num3 = newLength - block.Offset;
                        if (num3 > 0L)
                        {
                            block.Stream.SetLength(num3);
                            index++;
                        }
                    }
                    for (int i = index; i < this._memoryStreamList.Count; i++)
                    {
                        this._memoryStreamList[i].Stream.Close();
                    }
                    this._memoryStreamList.RemoveRange(index, this._memoryStreamList.Count - index);
                }
                this._currentStreamLength = newLength;
                if (this._currentStreamPosition > this._currentStreamLength)
                {
                    this._currentStreamPosition = this._currentStreamLength;
                }
            }
            this.SwitchModeIfNecessary();
        }

        private long SkipWrite(Stream targetStream, long currentPos, long offset)
        {
            long num = offset - currentPos;
            if (num > 0L)
            {
                byte[] buffer = new byte[Math.Min(0x80000L, num)];
                while (num > 0L)
                {
                    int count = (int)Math.Min(num, (long)buffer.Length);
                    targetStream.Write(buffer, 0, count);
                    num -= count;
                }
            }
            return offset;
        }

        private void SwitchModeIfNecessary()
        {
            if (this._isolatedStorageMode)
            {
                if (this._isolatedStorageStream.Length < this._lowWaterMark)
                {
                    if (this._isolatedStorageStream.Length > 0L)
                    {
                        MemoryStreamBlock item = new MemoryStreamBlock(this._trackingMemoryStreamFactory.Create((int)this._isolatedStorageStream.Length), 0L);
                        this._isolatedStorageStream.Seek(0L, SeekOrigin.Begin);
                        item.Stream.Seek(0L, SeekOrigin.Begin);
                        PackagingUtilities.CopyStream(this._isolatedStorageStream, item.Stream, 0x7fffffffffffffffL, 0x80000);
                        this._memoryStreamList.Add(item);
                    }
                    this._isolatedStorageMode = false;
                    this._isolatedStorageStream.SetLength(0L);
                    this._isolatedStorageStream.Flush();
                }
            }
            else if (this._trackingMemoryStreamFactory.CurrentMemoryConsumption > this._highWaterMark)
            {
                this.EnsureIsolatedStoreStream();
                this.CopyMemoryBlocksToStream(this._isolatedStorageStream);
                this._isolatedStorageMode = true;
                foreach (MemoryStreamBlock block2 in this._memoryStreamList)
                {
                    block2.Stream.Close();
                }
                this._memoryStreamList.Clear();
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            PackagingUtilities.VerifyStreamWriteArgs(this, buffer, offset, count);
            if (count != 0)
            {
                if (this._isolatedStorageMode)
                {
                    this._isolatedStorageStream.Seek(this._currentStreamPosition, SeekOrigin.Begin);
                    this._isolatedStorageStream.Write(buffer, offset, count);
                    this._currentStreamPosition += count;
                }
                else
                {
                    this.WriteAndCollapseBlocks(buffer, offset, count);
                }
                this._currentStreamLength = Math.Max(this._currentStreamLength, this._currentStreamPosition);
                this.SwitchModeIfNecessary();
            }
        }

        private void WriteAndCollapseBlocks(byte[] buffer, int offset, int count)
        {
            int index = this._memoryStreamList.BinarySearch(this.GetSearchBlockForOffset(this._currentStreamPosition));
            bool flag = false;
            MemoryStreamBlock item = null;
            MemoryStreamBlock memStreamBlock = null;
            if (index < 0)
            {
                index = ~index;
                if (index != 0)
                {
                    memStreamBlock = this._memoryStreamList[index - 1];
                }
                if (this.CanCollapseWithPreviousBlock(memStreamBlock, this._currentStreamPosition, (long)count))
                {
                    memStreamBlock.Stream.Seek(0L, SeekOrigin.End);
                    this.SkipWrite(memStreamBlock.Stream, memStreamBlock.EndOffset, this._currentStreamPosition);
                    memStreamBlock.Stream.Write(buffer, offset, count);
                    flag = true;
                }
            }
            else
            {
                memStreamBlock = this._memoryStreamList[index];
                if ((memStreamBlock.Stream.Length + count) <= 0x7fffffffL)
                {
                    memStreamBlock.Stream.Seek(this._currentStreamPosition - memStreamBlock.Offset, SeekOrigin.Begin);
                    memStreamBlock.Stream.Write(buffer, offset, count);
                    flag = true;
                    index++;
                }
                else
                {
                    memStreamBlock.Stream.SetLength(this._currentStreamPosition - item.Offset);
                }
            }
            if (!flag)
            {
                memStreamBlock = this.ConstructMemoryStreamFromWriteRequest(buffer, this._currentStreamPosition, count, offset);
                this._memoryStreamList.Insert(index, memStreamBlock);
                index++;
            }
            this._currentStreamPosition += count;
            int num2 = index;
            while (num2 < this._memoryStreamList.Count)
            {
                if (this._memoryStreamList[num2].EndOffset > this._currentStreamPosition)
                {
                    break;
                }
                this._memoryStreamList[num2].Stream.Close();
                num2++;
            }
            if ((num2 - index) > 0)
            {
                this._memoryStreamList.RemoveRange(index, num2 - index);
            }
            long num3 = -1L;
            if (index < this._memoryStreamList.Count)
            {
                item = this._memoryStreamList[index];
                num3 = this._currentStreamPosition - item.Offset;
            }
            else
            {
                item = null;
            }
            if (num3 <= 0L)
            {
                if ((item != null) && this.CanCollapseWithPreviousBlock(memStreamBlock, item.Offset, item.Stream.Length))
                {
                    this._memoryStreamList.RemoveAt(index);
                    memStreamBlock.Stream.Seek(0L, SeekOrigin.End);
                    this.SkipWrite(memStreamBlock.Stream, this._currentStreamPosition, item.Offset);
                    memStreamBlock.Stream.Write(item.Stream.GetBuffer(), 0, (int)item.Stream.Length);
                }
            }
            else
            {
                this._memoryStreamList.RemoveAt(index);
                int num4 = (int)(item.Stream.Length - num3);
                if ((memStreamBlock.Stream.Length + num4) <= 0x7fffffffL)
                {
                    memStreamBlock.Stream.Seek(0L, SeekOrigin.End);
                    memStreamBlock.Stream.Write(item.Stream.GetBuffer(), (int)num3, num4);
                }
                else
                {
                    item = this.ConstructMemoryStreamFromWriteRequest(item.Stream.GetBuffer(), this._currentStreamPosition, num4, (int)num3);
                    this._memoryStreamList.Insert(index, item);
                }
            }
        }

        internal void WriteToStream(Stream stream)
        {
            if (this._isolatedStorageMode)
            {
                this._isolatedStorageStream.Seek(0L, SeekOrigin.Begin);
                PackagingUtilities.CopyStream(this._isolatedStorageStream, stream, 0x7fffffffffffffffL, 0x80000);
            }
            else
            {
                this.CopyMemoryBlocksToStream(stream);
            }
        }

        // Properties
        public override bool CanRead
        {
            get
            {
                return !this._disposedFlag;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return !this._disposedFlag;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return !this._disposedFlag;
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

        internal List<MemoryStreamBlock> MemoryBlockCollection
        {
            get
            {
                this.CheckDisposed();
                return this._memoryStreamList;
            }
        }

        internal long MemoryConsumption
        {
            get
            {
                this.CheckDisposed();
                return this._trackingMemoryStreamFactory.CurrentMemoryConsumption;
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
