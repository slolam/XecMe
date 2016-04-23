using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIOCentralDirectoryDigitalSignature
    {
        // Fields
        private const long _fixedMinimalRecordSize = 6L;
        private const uint _signatureConstant = 0x5054b50;

        // Methods
        private ZipIOCentralDirectoryDigitalSignature()
        {
        }

        internal static ZipIOCentralDirectoryDigitalSignature ParseRecord(BinaryReader reader)
        {
            if (((reader.BaseStream.Length - reader.BaseStream.Position) >= 6L) && (reader.ReadUInt32() == 0x5054b50))
            {
                throw new NotSupportedException(SR.Get("ZipNotSupportedSignedArchive"));
            }
            return null;
        }
    }


    internal class ZipIOCentralDirectoryFileHeader
    {
        // Fields
        private uint _compressedSize;
        private ushort _compressionMethod;
        private const byte _constantUpperVersionMadeByMsDos = 0;
        private uint _crc32;
        private ushort _diskNumberStart;
        private Encoding _encoding;
        private uint _externalFileAttributes;
        private ZipIOExtraField _extraField;
        private ushort _extraFieldLength;
        private byte[] _fileComment;
        private ushort _fileCommentLength;
        private byte[] _fileName;
        private ushort _fileNameLength;
        private const int _fixedMinimalRecordSize = 0x2e;
        private ushort _generalPurposeBitFlag;
        private ushort _internalFileAttributes;
        private uint _lastModFileDateTime;
        private uint _relativeOffsetOfLocalHeader;
        private uint _signature = 0x2014b50;
        private const uint _signatureConstant = 0x2014b50;
        private const ushort _streamingBitMask = 8;
        private string _stringFileName;
        private uint _uncompressedSize;
        private ushort _versionMadeBy;
        private ushort _versionNeededToExtract;

        // Methods
        private ZipIOCentralDirectoryFileHeader(Encoding encoding)
        {
            this._encoding = encoding;
        }

        private bool CheckIfUpdateNeeded(ZipIOLocalFileBlock fileBlock)
        {
            bool flag2 = 0 != (fileBlock.GeneralPurposeBitFlag & 8);
            bool flag = 0 != (this._generalPurposeBitFlag & 8);
            if ((flag2 || !flag) && ((((this._signature == 0x2014b50) && (this._versionNeededToExtract == fileBlock.VersionNeededToExtract)) && ((this._generalPurposeBitFlag == fileBlock.GeneralPurposeBitFlag) && (this._compressionMethod == fileBlock.CompressionMethod))) && (((this._crc32 == fileBlock.Crc32) && (this.CompressedSize == fileBlock.CompressedSize)) && (this.UncompressedSize == fileBlock.UncompressedSize))))
            {
                return (this.OffsetOfLocalHeader != fileBlock.Offset);
            }
            return true;
        }

        internal static ZipIOCentralDirectoryFileHeader CreateNew(Encoding encoding, ZipIOLocalFileBlock fileBlock)
        {
            ZipIOCentralDirectoryFileHeader header = new ZipIOCentralDirectoryFileHeader(encoding);
            header._fileCommentLength = 0;
            header._fileComment = null;
            header._diskNumberStart = 0;
            header._internalFileAttributes = 0;
            header._externalFileAttributes = 0;
            header._versionMadeBy = 0x2d;
            header._extraField = ZipIOExtraField.CreateNew(false);
            header.UpdateFromLocalFileBlock(fileBlock);
            return header;
        }

        internal void MoveReference(long shiftSize)
        {
            this.UpdateZip64Structures(this.CompressedSize, this.UncompressedSize, this.OffsetOfLocalHeader + shiftSize);
        }

        internal static ZipIOCentralDirectoryFileHeader ParseRecord(BinaryReader reader, Encoding encoding)
        {
            ZipIOCentralDirectoryFileHeader header = new ZipIOCentralDirectoryFileHeader(encoding);
            header._signature = reader.ReadUInt32();
            header._versionMadeBy = reader.ReadUInt16();
            header._versionNeededToExtract = reader.ReadUInt16();
            header._generalPurposeBitFlag = reader.ReadUInt16();
            header._compressionMethod = reader.ReadUInt16();
            header._lastModFileDateTime = reader.ReadUInt32();
            header._crc32 = reader.ReadUInt32();
            header._compressedSize = reader.ReadUInt32();
            header._uncompressedSize = reader.ReadUInt32();
            header._fileNameLength = reader.ReadUInt16();
            header._extraFieldLength = reader.ReadUInt16();
            header._fileCommentLength = reader.ReadUInt16();
            header._diskNumberStart = reader.ReadUInt16();
            header._internalFileAttributes = reader.ReadUInt16();
            header._externalFileAttributes = reader.ReadUInt32();
            header._relativeOffsetOfLocalHeader = reader.ReadUInt32();
            header._fileName = reader.ReadBytes(header._fileNameLength);
            ZipIOZip64ExtraFieldUsage none = ZipIOZip64ExtraFieldUsage.None;
            if (header._versionNeededToExtract >= 0x2d)
            {
                if (header._compressedSize == uint.MaxValue)
                {
                    none |= ZipIOZip64ExtraFieldUsage.CompressedSize;
                }
                if (header._uncompressedSize == uint.MaxValue)
                {
                    none |= ZipIOZip64ExtraFieldUsage.UncompressedSize;
                }
                if (header._relativeOffsetOfLocalHeader == uint.MaxValue)
                {
                    none |= ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader;
                }
                if (header._diskNumberStart == 0xffff)
                {
                    none |= ZipIOZip64ExtraFieldUsage.DiskNumber;
                }
            }
            header._extraField = ZipIOExtraField.ParseRecord(reader, none, header._extraFieldLength);
            header._fileComment = reader.ReadBytes(header._fileCommentLength);
            header._stringFileName = ZipIOBlockManager.ValidateNormalizeFileName(encoding.GetString(header._fileName));
            header.Validate();
            return header;
        }

        internal void Save(BinaryWriter writer)
        {
            writer.Write((uint)0x2014b50);
            writer.Write(this._versionMadeBy);
            writer.Write(this._versionNeededToExtract);
            writer.Write(this._generalPurposeBitFlag);
            writer.Write(this._compressionMethod);
            writer.Write(this._lastModFileDateTime);
            writer.Write(this._crc32);
            writer.Write(this._compressedSize);
            writer.Write(this._uncompressedSize);
            writer.Write(this._fileNameLength);
            writer.Write(this._extraField.Size);
            writer.Write(this._fileCommentLength);
            writer.Write(this._diskNumberStart);
            writer.Write(this._internalFileAttributes);
            writer.Write(this._externalFileAttributes);
            writer.Write(this._relativeOffsetOfLocalHeader);
            writer.Write(this._fileName, 0, this._fileNameLength);
            this._extraField.Save(writer);
            if (this._fileCommentLength > 0)
            {
                writer.Write(this._fileComment, 0, this._fileCommentLength);
            }
        }

        private void UpdateFromLocalFileBlock(ZipIOLocalFileBlock fileBlock)
        {
            this._signature = 0x2014b50;
            this._generalPurposeBitFlag = fileBlock.GeneralPurposeBitFlag;
            this._compressionMethod = (ushort)fileBlock.CompressionMethod;
            this._lastModFileDateTime = fileBlock.LastModFileDateTime;
            this._crc32 = fileBlock.Crc32;
            this._fileNameLength = (ushort)fileBlock.FileName.Length;
            this._fileName = this._encoding.GetBytes(fileBlock.FileName);
            this._stringFileName = fileBlock.FileName;
            this.UpdateZip64Structures(fileBlock.CompressedSize, fileBlock.UncompressedSize, fileBlock.Offset);
            this._versionNeededToExtract = fileBlock.VersionNeededToExtract;
        }

        internal bool UpdateIfNeeded(ZipIOLocalFileBlock fileBlock)
        {
            if (this.CheckIfUpdateNeeded(fileBlock))
            {
                this.UpdateFromLocalFileBlock(fileBlock);
                return true;
            }
            return false;
        }

        private void UpdateZip64Structures(long compressedSize, long uncompressedSize, long offset)
        {
            if (((compressedSize >= 0xffffffffL) || (uncompressedSize >= 0xffffffffL)) || (offset >= 0xffffffffL))
            {
                this._extraField.CompressedSize = compressedSize;
                this._extraField.UncompressedSize = uncompressedSize;
                this._extraField.OffsetOfLocalHeader = offset;
                this._compressedSize = uint.MaxValue;
                this._uncompressedSize = uint.MaxValue;
                this._relativeOffsetOfLocalHeader = uint.MaxValue;
                this._versionNeededToExtract = 0x2d;
            }
            else
            {
                this._compressedSize = (uint)compressedSize;
                this._uncompressedSize = (uint)uncompressedSize;
                this._relativeOffsetOfLocalHeader = (uint)offset;
                this._extraField.Zip64ExtraFieldUsage = ZipIOZip64ExtraFieldUsage.None;
                this._versionNeededToExtract = (ushort)ZipIOBlockManager.CalcVersionNeededToExtractFromCompression((CompressionMethodEnum)this._compressionMethod);
            }
        }

        private void Validate()
        {
            if (this._signature != 0x2014b50)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (this.DiskNumberStart != 0)
            {
                throw new NotSupportedException(SR.Get("NotSupportedMultiDisk"));
            }
            if (this._fileNameLength != this._fileName.Length)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (this._extraFieldLength != this._extraField.Size)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            ZipArchive.VerifyVersionNeededToExtract(this._versionNeededToExtract);
            if ((this._versionNeededToExtract < 0x2d) && (this._extraField.Zip64ExtraFieldUsage != ZipIOZip64ExtraFieldUsage.None))
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (this._fileCommentLength != this._fileComment.Length)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if ((this._compressionMethod != 0) && (this._compressionMethod != 8))
            {
                throw new NotSupportedException(SR.Get("ZipNotSupportedCompressionMethod"));
            }
        }

        // Properties
        internal long CompressedSize
        {
            get
            {
                if ((this._extraField.Zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.CompressedSize) != ZipIOZip64ExtraFieldUsage.None)
                {
                    return this._extraField.CompressedSize;
                }
                return (long)this._compressedSize;
            }
        }

        internal CompressionMethodEnum CompressionMethod
        {
            get
            {
                return (CompressionMethodEnum)this._compressionMethod;
            }
        }

        internal uint Crc32
        {
            get
            {
                return this._crc32;
            }
        }

        internal uint DiskNumberStart
        {
            get
            {
                if ((this._extraField.Zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.DiskNumber) != ZipIOZip64ExtraFieldUsage.None)
                {
                    return this._extraField.DiskNumberOfFileStart;
                }
                return this._diskNumberStart;
            }
        }

        internal string FileName
        {
            get
            {
                return this._stringFileName;
            }
        }

        internal bool FolderFlag
        {
            get
            {
                return (((this._versionMadeBy & 0xff00) == 0) && ((this._externalFileAttributes & 0x10) != 0));
            }
        }

        internal ushort GeneralPurposeBitFlag
        {
            get
            {
                return this._generalPurposeBitFlag;
            }
        }

        internal long OffsetOfLocalHeader
        {
            get
            {
                if ((this._extraField.Zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader) != ZipIOZip64ExtraFieldUsage.None)
                {
                    return this._extraField.OffsetOfLocalHeader;
                }
                return (long)this._relativeOffsetOfLocalHeader;
            }
        }

        internal long Size
        {
            get
            {
                return (long)(((0x2e + this._fileNameLength) + this._extraField.Size) + this._fileCommentLength);
            }
        }

        internal long UncompressedSize
        {
            get
            {
                if ((this._extraField.Zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.UncompressedSize) != ZipIOZip64ExtraFieldUsage.None)
                {
                    return this._extraField.UncompressedSize;
                }
                return (long)this._uncompressedSize;
            }
        }

        internal ushort VersionNeededToExtract
        {
            get
            {
                return this._versionNeededToExtract;
            }
        }

        internal bool VolumeLabelFlag
        {
            get
            {
                return (((this._versionMadeBy & 0xff00) == 0) && ((this._externalFileAttributes & 8) != 0));
            }
        }
    }

}
