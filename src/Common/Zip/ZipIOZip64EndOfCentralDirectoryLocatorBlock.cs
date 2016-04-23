using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIOZip64EndOfCentralDirectoryLocatorBlock : IZipIOBlock
    {
        // Fields
        private ZipIOBlockManager _blockManager;
        private bool _dirtyFlag;
        private const int _fixedMinimalRecordSize = 20;
        private uint _numberOfTheDiskWithTheStartOfZip64EndOfCentralDirectory;
        private long _offset;
        private ulong _offsetOfStartOfZip64EndOfCentralDirectoryRecord;
        private uint _signature = 0x7064b50;
        private const uint _signatureConstant = 0x7064b50;
        private long _size;
        private uint _totalNumberOfDisks = 1;

        // Methods
        private ZipIOZip64EndOfCentralDirectoryLocatorBlock(ZipIOBlockManager blockManager)
        {
            this._blockManager = blockManager;
        }

        internal static ZipIOZip64EndOfCentralDirectoryLocatorBlock CreateNew(ZipIOBlockManager blockManager)
        {
            ZipIOZip64EndOfCentralDirectoryLocatorBlock block = new ZipIOZip64EndOfCentralDirectoryLocatorBlock(blockManager);
            block._offset = 0L;
            block._size = 0L;
            block._dirtyFlag = false;
            return block;
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
                if (this._size > 0L)
                {
                    this._dirtyFlag = true;
                }
            }
        }

        private void ParseRecord(BinaryReader reader, long position)
        {
            this._signature = reader.ReadUInt32();
            this._numberOfTheDiskWithTheStartOfZip64EndOfCentralDirectory = reader.ReadUInt32();
            this._offsetOfStartOfZip64EndOfCentralDirectoryRecord = reader.ReadUInt64();
            this._totalNumberOfDisks = reader.ReadUInt32();
            this._offset = position;
            this._size = 20L;
            this._dirtyFlag = false;
            this.Validate();
        }

        public PreSaveNotificationScanControlInstruction PreSaveNotification(long offset, long size)
        {
            return PreSaveNotificationScanControlInstruction.Stop;
        }

        public void Save()
        {
            if (this.GetDirtyFlag(true) && (this.Size > 0L))
            {
                BinaryWriter binaryWriter = this._blockManager.BinaryWriter;
                if (this._blockManager.Stream.Position != this._offset)
                {
                    this._blockManager.Stream.Seek(this._offset, SeekOrigin.Begin);
                }
                binaryWriter.Write((uint)0x7064b50);
                binaryWriter.Write(this._numberOfTheDiskWithTheStartOfZip64EndOfCentralDirectory);
                binaryWriter.Write(this._offsetOfStartOfZip64EndOfCentralDirectoryRecord);
                binaryWriter.Write(this._totalNumberOfDisks);
                binaryWriter.Flush();
            }
            this._dirtyFlag = false;
        }

        internal static ZipIOZip64EndOfCentralDirectoryLocatorBlock SeekableLoad(ZipIOBlockManager blockManager)
        {
            long offset = blockManager.EndOfCentralDirectoryBlock.Offset - 20L;
            blockManager.Stream.Seek(offset, SeekOrigin.Begin);
            ZipIOZip64EndOfCentralDirectoryLocatorBlock block = new ZipIOZip64EndOfCentralDirectoryLocatorBlock(blockManager);
            block.ParseRecord(blockManager.BinaryReader, offset);
            return block;
        }

        internal static bool SniffTheBlockSignature(ZipIOBlockManager blockManager)
        {
            long offset = blockManager.EndOfCentralDirectoryBlock.Offset - 20L;
            if ((offset < 0L) || ((offset + 4L) > blockManager.Stream.Length))
            {
                return false;
            }
            blockManager.Stream.Seek(offset, SeekOrigin.Begin);
            return (blockManager.BinaryReader.ReadUInt32() == 0x7064b50);
        }

        public void UpdateReferences(bool closingFlag)
        {
            if ((this._blockManager.IsCentralDirectoryBlockLoaded && (this._blockManager.Streaming || this._blockManager.CentralDirectoryBlock.GetDirtyFlag(closingFlag))) || this._blockManager.Zip64EndOfCentralDirectoryBlock.GetDirtyFlag(closingFlag))
            {
                if (this._blockManager.CentralDirectoryBlock.IsZip64BitRequiredForStoring)
                {
                    ulong offset = (ulong)this._blockManager.Zip64EndOfCentralDirectoryBlock.Offset;
                    if ((this._dirtyFlag || (offset != this._offsetOfStartOfZip64EndOfCentralDirectoryRecord)) || (20L != this._size))
                    {
                        this._offsetOfStartOfZip64EndOfCentralDirectoryRecord = offset;
                        this._size = 20L;
                        this._dirtyFlag = true;
                    }
                }
                else if (this._size != 0L)
                {
                    this._size = 0L;
                    this._dirtyFlag = true;
                }
            }
        }

        private void Validate()
        {
            if (this._offsetOfStartOfZip64EndOfCentralDirectoryRecord > 0x7fffffffffffffffL)
            {
                throw new NotSupportedException(SR.Get("Zip64StructuresTooLarge"));
            }
            if (this._signature != 0x7064b50)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if ((this._totalNumberOfDisks != 1) || (this._numberOfTheDiskWithTheStartOfZip64EndOfCentralDirectory != 0))
            {
                throw new NotSupportedException(SR.Get("NotSupportedMultiDisk"));
            }
            if (this._offset <= this._offsetOfStartOfZip64EndOfCentralDirectoryRecord)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if ((this._size != 20L) && (this._size != 0L))
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
        }

        // Properties
        public long Offset
        {
            get
            {
                return this._offset;
            }
        }

        internal long OffsetOfZip64EndOfCentralDirectoryRecord
        {
            get
            {
                return (long)this._offsetOfStartOfZip64EndOfCentralDirectoryRecord;
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
