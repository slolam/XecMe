using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIORawDataFileBlock : IZipIOBlock
    {
        // Fields
        private ZipIOBlockManager _blockManager;
        private SparseMemoryStream _cachePrefixStream;
        private bool _dirtyFlag;
        private long _offset;
        private long _persistedOffset;
        private long _size;

        // Methods
        private ZipIORawDataFileBlock(ZipIOBlockManager blockManager)
        {
            this._blockManager = blockManager;
        }

        internal static ZipIORawDataFileBlock Assign(ZipIOBlockManager blockManager, long offset, long size)
        {
            if (size <= 0L)
            {
                throw new ArgumentOutOfRangeException("size");
            }
            if (offset < 0L)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            ZipIORawDataFileBlock block = new ZipIORawDataFileBlock(blockManager);
            block._persistedOffset = offset;
            block._offset = offset;
            block._size = size;
            return block;
        }

        internal bool DiskImageContains(IZipIOBlock block)
        {
            return ((this._persistedOffset <= block.Offset) && ((this._persistedOffset + this._size) >= (block.Offset + block.Size)));
        }

        internal bool DiskImageContains(long offset)
        {
            return ((this._persistedOffset <= offset) && ((this._persistedOffset + this._size) > offset));
        }

        public bool GetDirtyFlag(bool closingFlag)
        {
            return this._dirtyFlag;
        }

        public void Move(long shiftSize)
        {
            if (shiftSize != 0L)
            {
                this._offset += shiftSize;
                this._dirtyFlag = true;
            }
        }

        public PreSaveNotificationScanControlInstruction PreSaveNotification(long offset, long size)
        {
            return ZipIOBlockManager.CommonPreSaveNotificationHandler(this._blockManager.Stream, offset, size, this._persistedOffset, this._size, ref this._cachePrefixStream);
        }

        public void Save()
        {
            if (this.GetDirtyFlag(true))
            {
                long moveBlockSourceOffset = this._persistedOffset;
                long moveBlockSize = this._size;
                long moveBlockTargetOffset = this._offset;
                if (this._cachePrefixStream != null)
                {
                    moveBlockSourceOffset += this._cachePrefixStream.Length;
                    moveBlockTargetOffset += this._cachePrefixStream.Length;
                    moveBlockSize -= this._cachePrefixStream.Length;
                }
                this._blockManager.MoveData(moveBlockSourceOffset, moveBlockTargetOffset, moveBlockSize);
                if (this._cachePrefixStream != null)
                {
                    if (this._blockManager.Stream.Position != this._offset)
                    {
                        this._blockManager.Stream.Seek(this._offset, SeekOrigin.Begin);
                    }
                    this._cachePrefixStream.WriteToStream(this._blockManager.Stream);
                    this._cachePrefixStream.Close();
                    this._cachePrefixStream = null;
                }
                this._persistedOffset = this._offset;
                this._dirtyFlag = false;
            }
        }

        internal void SplitIntoPrefixSuffix(IZipIOBlock block, out ZipIORawDataFileBlock prefixBlock, out ZipIORawDataFileBlock suffixBlock)
        {
            prefixBlock = null;
            suffixBlock = null;
            if (block.Offset > this._persistedOffset)
            {
                long offset = this._persistedOffset;
                long size = block.Offset - this._persistedOffset;
                prefixBlock = Assign(this._blockManager, offset, size);
            }
            if ((block.Offset + block.Size) < (this._persistedOffset + this._size))
            {
                long num = block.Offset + block.Size;
                long num2 = (this._persistedOffset + this._size) - num;
                suffixBlock = Assign(this._blockManager, num, num2);
            }
        }

        public void UpdateReferences(bool closingFlag)
        {
        }

        // Properties
        internal long DiskImageShift
        {
            get
            {
                return (this._offset - this._persistedOffset);
            }
        }

        public long Offset
        {
            get
            {
                return this._offset;
            }
        }

        public long Size
        {
            get
            {
                return this._size;
            }
        }
    }

}
