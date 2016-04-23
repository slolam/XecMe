using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIOEndOfCentralDirectoryBlock : IZipIOBlock
    {
        // Fields
        private ZipIOBlockManager _blockManager;
        private bool _dirtyFlag;
        private const int _fixedMinimalRecordSize = 0x16;
        private ushort _numberOfTheDiskWithTheStartOfTheCentralDirectory;
        private ushort _numberOfThisDisk;
        private long _offset;
        private uint _offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber;
        private static int _scanBlockSize = 0x1000;
        private uint _signature = 0x6054b50;
        private static byte[] _signatureBuffer = new byte[] { 80, 0x4b, 5, 6 };
        private const uint _signatureConstant = 0x6054b50;
        private uint _sizeOfTheCentralDirectory;
        private string _stringZipFileComment;
        private ushort _totalNumberOfEntriesInTheCentralDirectory;
        private ushort _totalNumberOfEntriesInTheCentralDirectoryOnThisDisk;
        private byte[] _zipFileComment;
        private ushort _zipFileCommentLength;

        // Methods
        private ZipIOEndOfCentralDirectoryBlock(ZipIOBlockManager blockManager)
        {
            this._blockManager = blockManager;
        }

        internal static ZipIOEndOfCentralDirectoryBlock CreateNew(ZipIOBlockManager blockManager, long offset)
        {
            ZipIOEndOfCentralDirectoryBlock block = new ZipIOEndOfCentralDirectoryBlock(blockManager);
            block._offset = offset;
            block._dirtyFlag = true;
            return block;
        }

        private static long FindPosition(Stream archiveStream)
        {
            byte[] buffer = new byte[_scanBlockSize + 0x16];
            long length = archiveStream.Length;
            for (long i = length; i > 0L; i -= _scanBlockSize)
            {
                long offset = Math.Max((long)0L, (long)(i - _scanBlockSize));
                archiveStream.Seek(offset, SeekOrigin.Begin);
                int num6 = PackagingUtilities.ReliableRead(archiveStream, buffer, 0, buffer.Length);
                long bufferOffsetFromEndOfStream = length - offset;
                for (int j = num6 - 0x16; j >= 0; j--)
                {
                    if (IsPositionMatched(j, buffer, bufferOffsetFromEndOfStream))
                    {
                        return (offset + j);
                    }
                }
            }
            throw new FileFormatException(SR.Get("CorruptedData"));
        }

        public bool GetDirtyFlag(bool closingFlag)
        {
            return this._dirtyFlag;
        }

        private static bool IsPositionMatched(int pos, byte[] buffer, long bufferOffsetFromEndOfStream)
        {
            for (int i = 0; i < _signatureBuffer.Length; i++)
            {
                if (_signatureBuffer[i] != buffer[pos + i])
                {
                    return false;
                }
            }
            long num3 = buffer[(pos + 0x16) - 2] + (buffer[(pos + 0x16) - 1] << 8);
            long num2 = (bufferOffsetFromEndOfStream - pos) - 0x16L;
            if (num2 != num3)
            {
                return false;
            }
            return true;
        }

        public void Move(long shiftSize)
        {
            if (shiftSize != 0L)
            {
                this._offset += shiftSize;
                this._dirtyFlag = true;
            }
        }

        private void ParseRecord(BinaryReader reader, long position)
        {
            this._signature = reader.ReadUInt32();
            this._numberOfThisDisk = reader.ReadUInt16();
            this._numberOfTheDiskWithTheStartOfTheCentralDirectory = reader.ReadUInt16();
            this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk = reader.ReadUInt16();
            this._totalNumberOfEntriesInTheCentralDirectory = reader.ReadUInt16();
            this._sizeOfTheCentralDirectory = reader.ReadUInt32();
            this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber = reader.ReadUInt32();
            this._zipFileCommentLength = reader.ReadUInt16();
            this._zipFileComment = reader.ReadBytes(this._zipFileCommentLength);
            this._stringZipFileComment = this._blockManager.Encoding.GetString(this._zipFileComment);
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
            if (this.GetDirtyFlag(true))
            {
                BinaryWriter binaryWriter = this._blockManager.BinaryWriter;
                if (!this._blockManager.Streaming && (this._blockManager.Stream.Position != this._offset))
                {
                    this._blockManager.Stream.Seek(this._offset, SeekOrigin.Begin);
                }
                binaryWriter.Write((uint)0x6054b50);
                binaryWriter.Write(this._numberOfThisDisk);
                binaryWriter.Write(this._numberOfTheDiskWithTheStartOfTheCentralDirectory);
                binaryWriter.Write(this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk);
                binaryWriter.Write(this._totalNumberOfEntriesInTheCentralDirectory);
                binaryWriter.Write(this._sizeOfTheCentralDirectory);
                binaryWriter.Write(this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber);
                binaryWriter.Write(this._zipFileCommentLength);
                if (this._zipFileCommentLength > 0)
                {
                    binaryWriter.Write(this._zipFileComment, 0, this._zipFileCommentLength);
                }
                binaryWriter.Flush();
                this._dirtyFlag = false;
            }
        }

        internal static ZipIOEndOfCentralDirectoryBlock SeekableLoad(ZipIOBlockManager blockManager)
        {
            long offset = FindPosition(blockManager.Stream);
            blockManager.Stream.Seek(offset, SeekOrigin.Begin);
            ZipIOEndOfCentralDirectoryBlock block = new ZipIOEndOfCentralDirectoryBlock(blockManager);
            block.ParseRecord(blockManager.BinaryReader, offset);
            return block;
        }

        public void UpdateReferences(bool closingFlag)
        {
            if (this._blockManager.IsCentralDirectoryBlockLoaded && ((this._blockManager.Streaming || this._blockManager.CentralDirectoryBlock.GetDirtyFlag(closingFlag)) || (this._blockManager.Zip64EndOfCentralDirectoryBlock.GetDirtyFlag(closingFlag) || this._blockManager.Zip64EndOfCentralDirectoryLocatorBlock.GetDirtyFlag(closingFlag))))
            {
                ushort count = 0xffff;
                uint maxValue = uint.MaxValue;
                uint offset = uint.MaxValue;
                ushort num5 = 0;
                ushort num4 = 0;
                if (!this._blockManager.CentralDirectoryBlock.IsZip64BitRequiredForStoring)
                {
                    count = (ushort)this._blockManager.CentralDirectoryBlock.Count;
                    maxValue = (uint)this._blockManager.CentralDirectoryBlock.Size;
                    offset = (uint)this._blockManager.CentralDirectoryBlock.Offset;
                }
                if (((this._dirtyFlag || (this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk != count)) || ((this._totalNumberOfEntriesInTheCentralDirectory != count) || (this._sizeOfTheCentralDirectory != maxValue))) || (((this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber != offset) || (this._numberOfTheDiskWithTheStartOfTheCentralDirectory != num5)) || (this._numberOfThisDisk != num4)))
                {
                    this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk = count;
                    this._totalNumberOfEntriesInTheCentralDirectory = count;
                    this._sizeOfTheCentralDirectory = maxValue;
                    this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber = offset;
                    this._numberOfTheDiskWithTheStartOfTheCentralDirectory = num5;
                    this._numberOfThisDisk = num4;
                    this._dirtyFlag = true;
                }
            }
        }

        private void Validate()
        {
            if (this._signature != 0x6054b50)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (this._zipFileCommentLength != this._zipFileComment.Length)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
        }

        internal void ValidateZip64TriggerValues()
        {
            if ((this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber > this._offset) || ((this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber == this._offset) && (this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk > 0)))
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (((this._numberOfThisDisk != 0) || (this._numberOfTheDiskWithTheStartOfTheCentralDirectory != 0)) || (this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk != this._totalNumberOfEntriesInTheCentralDirectory))
            {
                throw new NotSupportedException(SR.Get("NotSupportedMultiDisk"));
            }
        }

        // Properties
        internal bool ContainValuesHintingToPossibilityOfZip64
        {
            get
            {
                if ((((this._numberOfThisDisk != 0xffff) && (this._numberOfTheDiskWithTheStartOfTheCentralDirectory != 0xffff)) && ((this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk != 0xffff) && (this._totalNumberOfEntriesInTheCentralDirectory != 0xffff))) && (this._sizeOfTheCentralDirectory != uint.MaxValue))
                {
                    return (this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber == uint.MaxValue);
                }
                return true;
            }
        }

        internal uint NumberOfTheDiskWithTheStartOfTheCentralDirectory
        {
            get
            {
                return this._numberOfTheDiskWithTheStartOfTheCentralDirectory;
            }
        }

        internal uint NumberOfThisDisk
        {
            get
            {
                return this._numberOfThisDisk;
            }
        }

        public long Offset
        {
            get
            {
                return this._offset;
            }
        }

        internal uint OffsetOfStartOfCentralDirectory
        {
            get
            {
                return this._offsetOfStartOfCentralDirectoryWithRespectToTheStartingDiskNumber;
            }
        }

        public long Size
        {
            get
            {
                return (long)(0x16 + this._zipFileCommentLength);
            }
        }

        internal uint SizeOfTheCentralDirectory
        {
            get
            {
                return this._sizeOfTheCentralDirectory;
            }
        }

        internal uint TotalNumberOfEntriesInTheCentralDirectory
        {
            get
            {
                return this._totalNumberOfEntriesInTheCentralDirectory;
            }
        }

        internal uint TotalNumberOfEntriesInTheCentralDirectoryOnThisDisk
        {
            get
            {
                return this._totalNumberOfEntriesInTheCentralDirectoryOnThisDisk;
            }
        }
    }

}
