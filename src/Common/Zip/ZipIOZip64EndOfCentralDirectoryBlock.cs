using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIOZip64EndOfCentralDirectoryBlock : IZipIOBlock
    {
        // Fields
        private ZipIOBlockManager _blockManager;
        private bool _dirtyFlag;
        private const uint _fixedMinimalRecordSize = 0x38;
        private const uint _fixedMinimalValueOfSizeOfZip64EOCD = 0x2c;
        private uint _numberOfTheDiskWithTheStartOfTheCentralDirectory;
        private uint _numberOfThisDisk;
        private long _offset;
        private ulong _offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber;
        private uint _signature = 0x6064b50;
        private const uint _signatureConstant = 0x6064b50;
        private long _size;
        private ulong _sizeOfTheCentralDirectory;
        private ulong _sizeOfZip64EndOfCentralDirectory = 0x2cL;
        private ulong _totalNumberOfEntriesInTheCentralDirectory;
        private ulong _totalNumberOfEntriesInTheCentralDirectoryOnThisDisk;
        private ushort _versionMadeBy = 0x2d;
        private ushort _versionNeededToExtract = 0x2d;
        private byte[] _zip64ExtensibleDataSector;

        // Methods
        private ZipIOZip64EndOfCentralDirectoryBlock(ZipIOBlockManager blockManager)
        {
            this._blockManager = blockManager;
        }

        internal static ZipIOZip64EndOfCentralDirectoryBlock CreateNew(ZipIOBlockManager blockManager)
        {
            ZipIOZip64EndOfCentralDirectoryBlock block = new ZipIOZip64EndOfCentralDirectoryBlock(blockManager);
            block._size = 0L;
            block._offset = 0L;
            block._dirtyFlag = false;
            block.InitializeFromEndOfCentralDirectory(blockManager.EndOfCentralDirectoryBlock);
            return block;
        }

        public bool GetDirtyFlag(bool closingFlag)
        {
            return this._dirtyFlag;
        }

        private void InitializeFromEndOfCentralDirectory(ZipIOEndOfCentralDirectoryBlock zipIoEocd)
        {
            this._numberOfThisDisk = zipIoEocd.NumberOfThisDisk;
            this._numberOfTheDiskWithTheStartOfTheCentralDirectory = zipIoEocd.NumberOfTheDiskWithTheStartOfTheCentralDirectory;
            this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk = zipIoEocd.TotalNumberOfEntriesInTheCentralDirectoryOnThisDisk;
            this._totalNumberOfEntriesInTheCentralDirectory = zipIoEocd.TotalNumberOfEntriesInTheCentralDirectory;
            this._sizeOfTheCentralDirectory = zipIoEocd.SizeOfTheCentralDirectory;
            this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber = zipIoEocd.OffsetOfStartOfCentralDirectory;
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

        private void OverrideValuesBasedOnEndOfCentralDirectory(ZipIOEndOfCentralDirectoryBlock zipIoEocd)
        {
            if (zipIoEocd.NumberOfThisDisk < 0xffff)
            {
                this._numberOfThisDisk = zipIoEocd.NumberOfThisDisk;
            }
            if (zipIoEocd.NumberOfTheDiskWithTheStartOfTheCentralDirectory < 0xffff)
            {
                this._numberOfTheDiskWithTheStartOfTheCentralDirectory = zipIoEocd.NumberOfTheDiskWithTheStartOfTheCentralDirectory;
            }
            if (zipIoEocd.TotalNumberOfEntriesInTheCentralDirectoryOnThisDisk < 0xffff)
            {
                this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk = zipIoEocd.TotalNumberOfEntriesInTheCentralDirectoryOnThisDisk;
            }
            if (zipIoEocd.TotalNumberOfEntriesInTheCentralDirectory < 0xffff)
            {
                this._totalNumberOfEntriesInTheCentralDirectory = zipIoEocd.TotalNumberOfEntriesInTheCentralDirectory;
            }
            if (zipIoEocd.SizeOfTheCentralDirectory < uint.MaxValue)
            {
                this._sizeOfTheCentralDirectory = zipIoEocd.SizeOfTheCentralDirectory;
            }
            if (zipIoEocd.OffsetOfStartOfCentralDirectory < uint.MaxValue)
            {
                this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber = zipIoEocd.OffsetOfStartOfCentralDirectory;
            }
        }

        private void ParseRecord(BinaryReader reader, long position)
        {
            this._signature = reader.ReadUInt32();
            this._sizeOfZip64EndOfCentralDirectory = reader.ReadUInt64();
            this._versionMadeBy = reader.ReadUInt16();
            this._versionNeededToExtract = reader.ReadUInt16();
            this._numberOfThisDisk = reader.ReadUInt32();
            this._numberOfTheDiskWithTheStartOfTheCentralDirectory = reader.ReadUInt32();
            this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk = reader.ReadUInt64();
            this._totalNumberOfEntriesInTheCentralDirectory = reader.ReadUInt64();
            this._sizeOfTheCentralDirectory = reader.ReadUInt64();
            this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber = reader.ReadUInt64();
            if ((this._sizeOfZip64EndOfCentralDirectory < 0x2cL) || (this._sizeOfZip64EndOfCentralDirectory > 0xffffL))
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (this._sizeOfZip64EndOfCentralDirectory > 0x2cL)
            {
                this._zip64ExtensibleDataSector = reader.ReadBytes((int)(this._sizeOfZip64EndOfCentralDirectory - ((ulong)0x2cL)));
            }
            this.OverrideValuesBasedOnEndOfCentralDirectory(this._blockManager.EndOfCentralDirectoryBlock);
            this._size = (((long)this._sizeOfZip64EndOfCentralDirectory) + 0x38L) - 0x2cL;
            this._offset = position;
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
                binaryWriter.Write((uint)0x6064b50);
                binaryWriter.Write(this._sizeOfZip64EndOfCentralDirectory);
                binaryWriter.Write(this._versionMadeBy);
                binaryWriter.Write(this._versionNeededToExtract);
                binaryWriter.Write(this._numberOfThisDisk);
                binaryWriter.Write(this._numberOfTheDiskWithTheStartOfTheCentralDirectory);
                binaryWriter.Write(this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk);
                binaryWriter.Write(this._totalNumberOfEntriesInTheCentralDirectory);
                binaryWriter.Write(this._sizeOfTheCentralDirectory);
                binaryWriter.Write(this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber);
                if (this._sizeOfZip64EndOfCentralDirectory > 0x2cL)
                {
                    binaryWriter.Write(this._zip64ExtensibleDataSector, 0, (int)(this._sizeOfZip64EndOfCentralDirectory - ((ulong)0x2cL)));
                }
                binaryWriter.Flush();
            }
            this._dirtyFlag = false;
        }

        internal static ZipIOZip64EndOfCentralDirectoryBlock SeekableLoad(ZipIOBlockManager blockManager)
        {
            long offset = blockManager.Zip64EndOfCentralDirectoryLocatorBlock.OffsetOfZip64EndOfCentralDirectoryRecord;
            ZipIOZip64EndOfCentralDirectoryBlock block = new ZipIOZip64EndOfCentralDirectoryBlock(blockManager);
            blockManager.Stream.Seek(offset, SeekOrigin.Begin);
            block.ParseRecord(blockManager.BinaryReader, offset);
            return block;
        }

        public void UpdateReferences(bool closingFlag)
        {
            if (this._blockManager.IsCentralDirectoryBlockLoaded && (this._blockManager.Streaming || this._blockManager.CentralDirectoryBlock.GetDirtyFlag(closingFlag)))
            {
                if (this._blockManager.CentralDirectoryBlock.IsZip64BitRequiredForStoring)
                {
                    ulong count = (ulong)this._blockManager.CentralDirectoryBlock.Count;
                    ulong size = (ulong)this._blockManager.CentralDirectoryBlock.Size;
                    ulong offset = (ulong)this._blockManager.CentralDirectoryBlock.Offset;
                    long num2 = (((long)this._sizeOfZip64EndOfCentralDirectory) + 0x38L) - 0x2cL;
                    if (((this._dirtyFlag || (this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk != count)) || ((this._totalNumberOfEntriesInTheCentralDirectory != count) || (this._sizeOfTheCentralDirectory != size))) || ((this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber != offset) || (this._size != num2)))
                    {
                        this._versionMadeBy = 0x2d;
                        this._versionNeededToExtract = 0x2d;
                        this._numberOfThisDisk = 0;
                        this._numberOfTheDiskWithTheStartOfTheCentralDirectory = 0;
                        this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk = count;
                        this._totalNumberOfEntriesInTheCentralDirectory = count;
                        this._sizeOfTheCentralDirectory = size;
                        this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber = offset;
                        this._size = num2;
                        this._dirtyFlag = true;
                    }
                }
                else if (this._size != 0L)
                {
                    this._dirtyFlag = true;
                    this._size = 0L;
                }
            }
        }

        private void Validate()
        {
            if (this._signature != 0x6064b50)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (((this._numberOfThisDisk != 0) || (this._numberOfTheDiskWithTheStartOfTheCentralDirectory != 0)) || (this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk != this._totalNumberOfEntriesInTheCentralDirectory))
            {
                throw new NotSupportedException(SR.Get("NotSupportedMultiDisk"));
            }
            ZipArchive.VerifyVersionNeededToExtract(this._versionNeededToExtract);
            if (this._versionNeededToExtract != 0x2d)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (((this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk > 0x7fffffffL) || (this._totalNumberOfEntriesInTheCentralDirectory > 0x7fffffffL)) || ((this._sizeOfTheCentralDirectory > 0x7fffffffffffffffL) || (this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber > 0x7fffffffffffffffL)))
            {
                throw new NotSupportedException(SR.Get("Zip64StructuresTooLarge"));
            }
            ulong length = 0L;
            if (this._zip64ExtensibleDataSector != null)
            {
                length = (ulong)this._zip64ExtensibleDataSector.Length;
            }
            if ((this._sizeOfZip64EndOfCentralDirectory - ((ulong)0x2cL)) != length)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (this._size < 0x38L)
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

        internal long OffsetOfStartOfCentralDirectory
        {
            get
            {
                return (long)this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber;
            }
        }

        public long Size
        {
            get
            {
                return this._size;
            }
        }

        internal long SizeOfCentralDirectory
        {
            get
            {
                return (long)this._sizeOfTheCentralDirectory;
            }
        }

        internal int TotalNumberOfEntriesInTheCentralDirectory
        {
            get
            {
                return (int)this._totalNumberOfEntriesInTheCentralDirectory;
            }
        }
    }

}
