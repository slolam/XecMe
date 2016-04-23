using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIOLocalFileBlock : IZipIOBlock, IDisposable
    {
        // Fields
        private ZipIOBlockManager _blockManager;
        private ProgressiveCrcCalculatingStream _crcCalculatingStream;
        private Stream _deflateStream;
        private bool _dirtyFlag;
        private bool _disposedFlag;
        private ArrayList _exposedPublicStreams;
        private ZipIOFileItemStream _fileItemStream;
        private bool _folderFlag;
        private const int _initialExposedPublicStreamsCollectionSize = 5;
        private ZipIOLocalFileDataDescriptor _localFileDataDescriptor;
        private ZipIOLocalFileHeader _localFileHeader;
        private bool _localFileHeaderSaved;
        private long _offset;
        private bool _volumeLabelFlag;

        // Methods
        private ZipIOLocalFileBlock(ZipIOBlockManager blockManager, bool folderFlag, bool volumeLabelFlag)
        {
            this._blockManager = blockManager;
            this._folderFlag = folderFlag;
            this._volumeLabelFlag = volumeLabelFlag;
        }

        internal void CheckDisposed()
        {
            if (this._disposedFlag)
            {
                throw new ObjectDisposedException(null, SR.Get("ZipFileItemDisposed"));
            }
        }

        private static void CheckFileAccessParameter(Stream stream, FileAccess access)
        {
            switch (access)
            {
                case FileAccess.Read:
                    if (!stream.CanRead)
                    {
                        throw new ArgumentException(SR.Get("CanNotReadInWriteOnlyMode"));
                    }
                    break;

                case FileAccess.Write:
                    if (!stream.CanWrite)
                    {
                        throw new ArgumentException(SR.Get("CanNotWriteInReadOnlyMode"));
                    }
                    break;

                case FileAccess.ReadWrite:
                    if (!stream.CanRead || !stream.CanWrite)
                    {
                        throw new ArgumentException(SR.Get("CanNotReadWriteInReadOnlyWriteOnlyMode"));
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException("access");
            }
        }

        private void CloseExposedStreams()
        {
            if (this._exposedPublicStreams != null)
            {
                for (int i = this._exposedPublicStreams.Count - 1; i >= 0; i--)
                {
                    ((ZipIOModeEnforcingStream)this._exposedPublicStreams[i]).Close();
                }
            }
        }

        internal static ZipIOLocalFileBlock CreateNew(ZipIOBlockManager blockManager, string fileName, CompressionMethodEnum compressionMethod, DeflateOptionEnum deflateOption)
        {
            ZipIOLocalFileBlock block = new ZipIOLocalFileBlock(blockManager, false, false);
            block._localFileHeader = ZipIOLocalFileHeader.CreateNew(fileName, blockManager.Encoding, compressionMethod, deflateOption, blockManager.Streaming);
            if (blockManager.Streaming)
            {
                block._localFileDataDescriptor = ZipIOLocalFileDataDescriptor.CreateNew();
            }
            block._offset = 0L;
            block._dirtyFlag = true;
            block._fileItemStream = new ZipIOFileItemStream(blockManager, block, block._offset + block._localFileHeader.Size, 0L);
            if (compressionMethod == CompressionMethodEnum.Deflated)
            {
                block._deflateStream = new CompressStream(block._fileItemStream, 0L, true);
                block._crcCalculatingStream = new ProgressiveCrcCalculatingStream(blockManager, block._deflateStream);
                return block;
            }
            block._crcCalculatingStream = new ProgressiveCrcCalculatingStream(blockManager, block._fileItemStream);
            return block;
        }

        internal void DeregisterExposedStream(Stream exposedStream)
        {
            this._exposedPublicStreams.Remove(exposedStream);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this._disposedFlag)
            {
                try
                {
                    this.CloseExposedStreams();
                    this._crcCalculatingStream.Close();
                    if (this._deflateStream != null)
                    {
                        this._deflateStream.Close();
                    }
                    this._fileItemStream.Close();
                }
                finally
                {
                    this._disposedFlag = true;
                    this._crcCalculatingStream = null;
                    this._deflateStream = null;
                    this._fileItemStream = null;
                }
            }
        }

        private void FlushExposedStreams()
        {
            this._crcCalculatingStream.Flush();
            if (((this._deflateStream != null) && !this._localFileHeader.StreamingCreationFlag) && ((this._exposedPublicStreams == null) || (this._exposedPublicStreams.Count == 0)))
            {
                ((CompressStream)this._deflateStream).Reset();
            }
        }

        public bool GetDirtyFlag(bool closingFlag)
        {
            this.CheckDisposed();
            bool flag = false;
            if (this._deflateStream != null)
            {
                flag = ((CompressStream)this._deflateStream).IsDirty(closingFlag);
            }
            if (!this._dirtyFlag && !this._fileItemStream.DirtyFlag)
            {
                return flag;
            }
            return true;
        }

        internal Stream GetStream(FileMode mode, FileAccess access)
        {
            this.CheckDisposed();
            CheckFileAccessParameter(this._blockManager.Stream, access);
            switch (mode)
            {
                case FileMode.CreateNew:
                    throw new ArgumentException(SR.Get("FileModeUnsupported", new object[] { "CreateNew" }));

                case FileMode.Create:
                    if (!this._blockManager.Stream.CanWrite)
                    {
                        throw new InvalidOperationException(SR.Get("CanNotWriteInReadOnlyMode"));
                    }
                    if ((this._crcCalculatingStream != null) && !this._blockManager.Streaming)
                    {
                        this._crcCalculatingStream.SetLength(0L);
                    }
                    break;

                case FileMode.Open:
                case FileMode.OpenOrCreate:
                    break;

                case FileMode.Truncate:
                    throw new ArgumentException(SR.Get("FileModeUnsupported", new object[] { "Truncate" }));

                case FileMode.Append:
                    throw new ArgumentException(SR.Get("FileModeUnsupported", new object[] { "Append" }));

                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
            if ((this._blockManager.Streaming && (this._exposedPublicStreams != null)) && (this._exposedPublicStreams.Count > 0))
            {
                return (Stream)this._exposedPublicStreams[0];
            }
            Stream exposedStream = new ZipIOModeEnforcingStream(this._crcCalculatingStream, access, this._blockManager, this);
            this.RegisterExposedStream(exposedStream);
            return exposedStream;
        }

        public void Move(long shiftSize)
        {
            this.CheckDisposed();
            if (shiftSize != 0L)
            {
                this._offset += shiftSize;
                this._fileItemStream.Move(shiftSize);
                this._dirtyFlag = true;
            }
        }

        private void ParseRecord(BinaryReader reader, string fileName, long position, ZipIOCentralDirectoryBlock centralDir, ZipIOCentralDirectoryFileHeader centralDirFileHeader)
        {
            this.CheckDisposed();
            this._localFileHeader = ZipIOLocalFileHeader.ParseRecord(reader, this._blockManager.Encoding);
            if (this._localFileHeader.StreamingCreationFlag)
            {
                this._blockManager.Stream.Seek(centralDirFileHeader.CompressedSize, SeekOrigin.Current);
                this._localFileDataDescriptor = ZipIOLocalFileDataDescriptor.ParseRecord(reader, centralDirFileHeader.CompressedSize, centralDirFileHeader.UncompressedSize, centralDirFileHeader.Crc32, this._localFileHeader.VersionNeededToExtract);
            }
            else
            {
                this._localFileDataDescriptor = null;
            }
            this._offset = position;
            this._dirtyFlag = false;
            this._fileItemStream = new ZipIOFileItemStream(this._blockManager, this, position + this._localFileHeader.Size, centralDirFileHeader.CompressedSize);
            if (this._localFileHeader.CompressionMethod == CompressionMethodEnum.Deflated)
            {
                this._deflateStream = new CompressStream(this._fileItemStream, centralDirFileHeader.UncompressedSize);
                this._crcCalculatingStream = new ProgressiveCrcCalculatingStream(this._blockManager, this._deflateStream, this.Crc32);
            }
            else
            {
                if (this._localFileHeader.CompressionMethod != CompressionMethodEnum.Stored)
                {
                    throw new NotSupportedException(SR.Get("ZipNotSupportedCompressionMethod"));
                }
                this._crcCalculatingStream = new ProgressiveCrcCalculatingStream(this._blockManager, this._fileItemStream, this.Crc32);
            }
            this.Validate(fileName, centralDir, centralDirFileHeader);
        }

        public PreSaveNotificationScanControlInstruction PreSaveNotification(long offset, long size)
        {
            this.CheckDisposed();
            return this._fileItemStream.PreSaveNotification(offset, size);
        }

        private void RegisterExposedStream(Stream exposedStream)
        {
            if (this._exposedPublicStreams == null)
            {
                this._exposedPublicStreams = new ArrayList(5);
            }
            this._exposedPublicStreams.Add(exposedStream);
        }

        public void Save()
        {
            this.CheckDisposed();
            if (this.GetDirtyFlag(true))
            {
                this._fileItemStream.PreSaveNotification(this._offset, this._localFileHeader.Size);
                BinaryWriter binaryWriter = this._blockManager.BinaryWriter;
                if (this._blockManager.Stream.Position != this._offset)
                {
                    this._blockManager.Stream.Seek(this._offset, SeekOrigin.Begin);
                }
                this._localFileHeader.Save(binaryWriter);
                this._fileItemStream.Save();
                this._dirtyFlag = false;
            }
        }

        internal void SaveStreaming(bool closingFlag)
        {
            this.CheckDisposed();
            if (this.GetDirtyFlag(closingFlag))
            {
                BinaryWriter binaryWriter = this._blockManager.BinaryWriter;
                if (!this._localFileHeaderSaved)
                {
                    this._offset = this._blockManager.Stream.Position;
                    this._localFileHeader.Save(binaryWriter);
                    this._localFileHeaderSaved = true;
                }
                this.FlushExposedStreams();
                this._fileItemStream.SaveStreaming();
                if (closingFlag)
                {
                    this._localFileDataDescriptor.UncompressedSize = this._crcCalculatingStream.Length;
                    this._localFileDataDescriptor.Crc32 = this._crcCalculatingStream.CalculateCrc();
                    this.CloseExposedStreams();
                    if (this._deflateStream != null)
                    {
                        this._deflateStream.Close();
                        this._fileItemStream.SaveStreaming();
                    }
                    this._localFileDataDescriptor.CompressedSize = this._fileItemStream.Length;
                    this._localFileDataDescriptor.Save(binaryWriter);
                    this._dirtyFlag = false;
                }
            }
        }

        internal static ZipIOLocalFileBlock SeekableLoad(ZipIOBlockManager blockManager, string fileName)
        {
            ZipIOCentralDirectoryBlock centralDirectoryBlock = blockManager.CentralDirectoryBlock;
            ZipIOCentralDirectoryFileHeader centralDirectoryFileHeader = centralDirectoryBlock.GetCentralDirectoryFileHeader(fileName);
            long offsetOfLocalHeader = centralDirectoryFileHeader.OffsetOfLocalHeader;
            bool folderFlag = centralDirectoryFileHeader.FolderFlag;
            bool volumeLabelFlag = centralDirectoryFileHeader.VolumeLabelFlag;
            blockManager.Stream.Seek(offsetOfLocalHeader, SeekOrigin.Begin);
            ZipIOLocalFileBlock block = new ZipIOLocalFileBlock(blockManager, folderFlag, volumeLabelFlag);
            block.ParseRecord(blockManager.BinaryReader, fileName, offsetOfLocalHeader, centralDirectoryBlock, centralDirectoryFileHeader);
            return block;
        }

        public void UpdateReferences(bool closingFlag)
        {
            Invariant.Assert(!this._blockManager.Streaming);
            this.CheckDisposed();
            if (closingFlag)
            {
                this.CloseExposedStreams();
            }
            else
            {
                this.FlushExposedStreams();
            }
            if (this.GetDirtyFlag(closingFlag))
            {
                long size = this._localFileHeader.Size;
                long length = this._crcCalculatingStream.Length;
                this._localFileHeader.Crc32 = this._crcCalculatingStream.CalculateCrc();
                if (closingFlag)
                {
                    this._crcCalculatingStream.Close();
                    if (this._deflateStream != null)
                    {
                        this._deflateStream.Close();
                    }
                }
                if (this._fileItemStream.DataChanged)
                {
                    this._localFileHeader.LastModFileDateTime = ZipIOBlockManager.ToMsDosDateTime(DateTime.Now);
                }
                long compressedSize = this._fileItemStream.Length;
                this._localFileHeader.UpdateZip64Structures(compressedSize, length, this.Offset);
                this._localFileHeader.UpdatePadding(this._localFileHeader.Size - size);
                this._localFileHeader.StreamingCreationFlag = false;
                this._localFileDataDescriptor = null;
                this._fileItemStream.Move((this.Offset + this._localFileHeader.Size) - this._fileItemStream.Offset);
                this._dirtyFlag = true;
            }
        }

        private void Validate(string fileName, ZipIOCentralDirectoryBlock centralDir, ZipIOCentralDirectoryFileHeader centralDirFileHeader)
        {
            if (string.CompareOrdinal(this._localFileHeader.FileName, fileName) != 0)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if ((((this.VersionNeededToExtract != centralDirFileHeader.VersionNeededToExtract) || (this.GeneralPurposeBitFlag != centralDirFileHeader.GeneralPurposeBitFlag)) || ((this.CompressedSize != centralDirFileHeader.CompressedSize) || (this.UncompressedSize != centralDirFileHeader.UncompressedSize))) || ((this.CompressionMethod != centralDirFileHeader.CompressionMethod) || (this.Crc32 != centralDirFileHeader.Crc32)))
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if ((this.Offset + this.Size) > centralDir.Offset)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
        }

        // Properties
        internal long CompressedSize
        {
            get
            {
                this.CheckDisposed();
                if (this._localFileHeader.StreamingCreationFlag)
                {
                    Invariant.Assert(this._localFileDataDescriptor != null);
                    return this._localFileDataDescriptor.CompressedSize;
                }
                return this._localFileHeader.CompressedSize;
            }
        }

        internal CompressionMethodEnum CompressionMethod
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.CompressionMethod;
            }
        }

        internal uint Crc32
        {
            get
            {
                this.CheckDisposed();
                if (this._localFileHeader.StreamingCreationFlag)
                {
                    Invariant.Assert(this._localFileDataDescriptor != null);
                    return this._localFileDataDescriptor.Crc32;
                }
                return this._localFileHeader.Crc32;
            }
        }

        internal DeflateOptionEnum DeflateOption
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.DeflateOption;
            }
        }

        internal string FileName
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.FileName;
            }
        }

        internal bool FolderFlag
        {
            get
            {
                this.CheckDisposed();
                return this._folderFlag;
            }
        }

        internal ushort GeneralPurposeBitFlag
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.GeneralPurposeBitFlag;
            }
        }

        internal uint LastModFileDateTime
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.LastModFileDateTime;
            }
        }

        public long Offset
        {
            get
            {
                this.CheckDisposed();
                return this._offset;
            }
        }

        public long Size
        {
            get
            {
                this.CheckDisposed();
                long num = this._localFileHeader.Size + this._fileItemStream.Length;
                if (this._localFileDataDescriptor != null)
                {
                    num += this._localFileDataDescriptor.Size;
                }
                return num;
            }
        }

        internal long UncompressedSize
        {
            get
            {
                this.CheckDisposed();
                if (this._localFileHeader.StreamingCreationFlag)
                {
                    Invariant.Assert(this._localFileDataDescriptor != null);
                    return this._localFileDataDescriptor.UncompressedSize;
                }
                return this._localFileHeader.UncompressedSize;
            }
        }

        internal ushort VersionNeededToExtract
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.VersionNeededToExtract;
            }
        }

        internal bool VolumeLabelFlag
        {
            get
            {
                this.CheckDisposed();
                return this._volumeLabelFlag;
            }
        }
    }

    internal class ZipIOLocalFileDataDescriptor
    {
        // Fields
        private long _compressedSize;
        private uint _crc32;
        private const int _fixedMinimalRecordSizeWithoutSignature = 12;
        private const int _fixedMinimalRecordSizeWithoutSignatureZip64 = 20;
        private const int _fixedMinimalRecordSizeWithSignature = 0x10;
        private const int _fixedMinimalRecordSizeWithSignatureZip64 = 0x18;
        private uint _signature = 0x8074b50;
        private const uint _signatureConstant = 0x8074b50;
        private int _size;
        private long _uncompressedSize;

        // Methods
        internal static ZipIOLocalFileDataDescriptor CreateNew()
        {
            ZipIOLocalFileDataDescriptor descriptor = new ZipIOLocalFileDataDescriptor();
            descriptor._size = 0x18;
            return descriptor;
        }

        internal static ZipIOLocalFileDataDescriptor ParseRecord(BinaryReader reader, long compressedSizeFromCentralDir, long uncompressedSizeFromCentralDir, uint crc32FromCentralDir, ushort versionNeededToExtract)
        {
            ZipIOLocalFileDataDescriptor descriptor = new ZipIOLocalFileDataDescriptor();
            uint[] numArray = new uint[6];
            numArray[0] = reader.ReadUInt32();
            numArray[1] = reader.ReadUInt32();
            numArray[2] = reader.ReadUInt32();
            if (descriptor.TestMatch(0x8074b50, crc32FromCentralDir, compressedSizeFromCentralDir, uncompressedSizeFromCentralDir, 0x8074b50, numArray[0], (ulong)numArray[1], (ulong)numArray[2]))
            {
                descriptor._size = 12;
                return descriptor;
            }
            numArray[3] = reader.ReadUInt32();
            if (descriptor.TestMatch(0x8074b50, crc32FromCentralDir, compressedSizeFromCentralDir, uncompressedSizeFromCentralDir, numArray[0], numArray[1], (ulong)numArray[2], (ulong)numArray[3]))
            {
                descriptor._size = 0x10;
                return descriptor;
            }
            if (versionNeededToExtract < 0x2d)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            numArray[4] = reader.ReadUInt32();
            if (descriptor.TestMatch(0x8074b50, crc32FromCentralDir, compressedSizeFromCentralDir, uncompressedSizeFromCentralDir, 0x8074b50, numArray[0], ZipIOBlockManager.ConvertToUInt64(numArray[1], numArray[2]), ZipIOBlockManager.ConvertToUInt64(numArray[3], numArray[4])))
            {
                descriptor._size = 20;
                return descriptor;
            }
            numArray[5] = reader.ReadUInt32();
            if (!descriptor.TestMatch(0x8074b50, crc32FromCentralDir, compressedSizeFromCentralDir, uncompressedSizeFromCentralDir, numArray[0], numArray[1], ZipIOBlockManager.ConvertToUInt64(numArray[2], numArray[3]), ZipIOBlockManager.ConvertToUInt64(numArray[4], numArray[5])))
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            descriptor._size = 0x18;
            return descriptor;
        }

        internal void Save(BinaryWriter writer)
        {
            Invariant.Assert(this._size == 0x18);
            writer.Write((uint)0x8074b50);
            writer.Write(this._crc32);
            writer.Write((ulong)this._compressedSize);
            writer.Write((ulong)this._uncompressedSize);
        }

        private bool TestMatch(uint signature, uint crc32FromCentralDir, long compressedSizeFromCentralDir, long uncompressedSizeFromCentralDir, uint suspectSignature, uint suspectCrc32, ulong suspectCompressedSize, ulong suspectUncompressedSize)
        {
            if (((signature == suspectSignature) && (((ulong)compressedSizeFromCentralDir) == suspectCompressedSize)) && ((((ulong)uncompressedSizeFromCentralDir) == suspectUncompressedSize) && (crc32FromCentralDir == suspectCrc32)))
            {
                this._signature = suspectSignature;
                this._compressedSize = (long)suspectCompressedSize;
                this._uncompressedSize = (long)suspectUncompressedSize;
                this._crc32 = suspectCrc32;
                return true;
            }
            return false;
        }

        // Properties
        internal long CompressedSize
        {
            get
            {
                return this._compressedSize;
            }
            set
            {
                Invariant.Assert(value >= 0L, "CompressedSize must be non-negative");
                this._compressedSize = value;
            }
        }

        internal uint Crc32
        {
            get
            {
                return this._crc32;
            }
            set
            {
                this._crc32 = value;
            }
        }

        internal long Size
        {
            get
            {
                return (long)this._size;
            }
        }

        internal long UncompressedSize
        {
            get
            {
                return this._uncompressedSize;
            }
            set
            {
                Invariant.Assert(value >= 0L, "UncompressedSize must be non-negative");
                this._uncompressedSize = value;
            }
        }
    }


    internal class ZipIOLocalFileHeader
    {
        // Fields
        private uint _compressedSize;
        private ushort _compressionMethod;
        private uint _crc32;
        private ZipIOExtraField _extraField;
        private ushort _extraFieldLength;
        private byte[] _fileName;
        private ushort _fileNameLength;
        private const int _fixedMinimalRecordSize = 30;
        private ushort _generalPurposeBitFlag;
        private uint _lastModFileDateTime;
        private uint _signature = 0x4034b50;
        private const uint _signatureConstant = 0x4034b50;
        private string _stringFileName;
        private uint _uncompressedSize;
        private ushort _versionNeededToExtract;

        // Methods
        private ZipIOLocalFileHeader()
        {
        }

        internal static ZipIOLocalFileHeader CreateNew(string fileName, Encoding encoding, CompressionMethodEnum compressionMethod, DeflateOptionEnum deflateOption, bool streaming)
        {
            byte[] bytes = encoding.GetBytes(fileName);
            if (bytes.Length > ZipIOBlockManager.MaxFileNameSize)
            {
                throw new ArgumentOutOfRangeException("fileName");
            }
            ZipIOLocalFileHeader header = new ZipIOLocalFileHeader();
            header._signature = 0x4034b50;
            header._compressionMethod = (ushort)compressionMethod;
            if (streaming)
            {
                header._versionNeededToExtract = 0x2d;
            }
            else
            {
                header._versionNeededToExtract = (ushort)ZipIOBlockManager.CalcVersionNeededToExtractFromCompression(compressionMethod);
            }
            if (compressionMethod != CompressionMethodEnum.Stored)
            {
                header.DeflateOption = deflateOption;
            }
            if (streaming)
            {
                header.StreamingCreationFlag = true;
            }
            header._lastModFileDateTime = ZipIOBlockManager.ToMsDosDateTime(DateTime.Now);
            header._fileNameLength = (ushort)bytes.Length;
            header._fileName = bytes;
            header._extraField = ZipIOExtraField.CreateNew(!streaming);
            header._extraFieldLength = header._extraField.Size;
            header._stringFileName = fileName;
            return header;
        }

        internal static ZipIOLocalFileHeader ParseRecord(BinaryReader reader, Encoding encoding)
        {
            ZipIOLocalFileHeader header = new ZipIOLocalFileHeader();
            header._signature = reader.ReadUInt32();
            header._versionNeededToExtract = reader.ReadUInt16();
            header._generalPurposeBitFlag = reader.ReadUInt16();
            header._compressionMethod = reader.ReadUInt16();
            header._lastModFileDateTime = reader.ReadUInt32();
            header._crc32 = reader.ReadUInt32();
            header._compressedSize = reader.ReadUInt32();
            header._uncompressedSize = reader.ReadUInt32();
            header._fileNameLength = reader.ReadUInt16();
            header._extraFieldLength = reader.ReadUInt16();
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
            }
            header._extraField = ZipIOExtraField.ParseRecord(reader, none, header._extraFieldLength);
            header._stringFileName = ZipIOBlockManager.ValidateNormalizeFileName(encoding.GetString(header._fileName));
            header.Validate();
            return header;
        }

        internal void Save(BinaryWriter writer)
        {
            writer.Write((uint)0x4034b50);
            writer.Write(this._versionNeededToExtract);
            writer.Write(this._generalPurposeBitFlag);
            writer.Write(this._compressionMethod);
            writer.Write(this._lastModFileDateTime);
            writer.Write(this._crc32);
            writer.Write(this._compressedSize);
            writer.Write(this._uncompressedSize);
            writer.Write(this._fileNameLength);
            writer.Write(this._extraField.Size);
            if (this._fileNameLength > 0)
            {
                writer.Write(this._fileName, 0, this._fileNameLength);
            }
            this._extraField.Save(writer);
            writer.Flush();
        }

        internal void UpdatePadding(long headerSizeChange)
        {
            this._extraField.UpdatePadding(headerSizeChange);
        }

        internal void UpdateZip64Structures(long compressedSize, long uncompressedSize, long offset)
        {
            if (((compressedSize >= 0xffffffffL) || (uncompressedSize >= 0xffffffffL)) || (offset >= 0xffffffffL))
            {
                this._extraField.CompressedSize = compressedSize;
                this._extraField.UncompressedSize = uncompressedSize;
                this._compressedSize = uint.MaxValue;
                this._uncompressedSize = uint.MaxValue;
                this._versionNeededToExtract = 0x2d;
            }
            else
            {
                this._compressedSize = (uint)compressedSize;
                this._uncompressedSize = (uint)uncompressedSize;
                this._extraField.Zip64ExtraFieldUsage = ZipIOZip64ExtraFieldUsage.None;
                this._versionNeededToExtract = (ushort)ZipIOBlockManager.CalcVersionNeededToExtractFromCompression(this.CompressionMethod);
            }
        }

        private void Validate()
        {
            if (this._signature != 0x4034b50)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (this._fileNameLength != this._fileName.Length)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            ZipArchive.VerifyVersionNeededToExtract(this._versionNeededToExtract);
            if (this.EncryptedFlag)
            {
                throw new NotSupportedException(SR.Get("ZipNotSupportedEncryptedArchive"));
            }
            if ((this._versionNeededToExtract < 0x2d) && (this._extraField.Zip64ExtraFieldUsage != ZipIOZip64ExtraFieldUsage.None))
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if ((this._extraField.Zip64ExtraFieldUsage != ZipIOZip64ExtraFieldUsage.None) && (this._extraField.Zip64ExtraFieldUsage != (ZipIOZip64ExtraFieldUsage.CompressedSize | ZipIOZip64ExtraFieldUsage.UncompressedSize)))
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (this._extraFieldLength != this._extraField.Size)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if ((this.CompressionMethod != CompressionMethodEnum.Stored) && (this.CompressionMethod != CompressionMethodEnum.Deflated))
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
            set
            {
                this._crc32 = value;
            }
        }

        internal DeflateOptionEnum DeflateOption
        {
            get
            {
                if (this.CompressionMethod == CompressionMethodEnum.Deflated)
                {
                    return (DeflateOptionEnum)((byte)(this._generalPurposeBitFlag & 6));
                }
                return DeflateOptionEnum.None;
            }
            set
            {
                this._generalPurposeBitFlag = (ushort)(this._generalPurposeBitFlag & 0xfff9);
                this._generalPurposeBitFlag = (ushort)(this._generalPurposeBitFlag | ((ushort)value));
            }
        }

        private bool EncryptedFlag
        {
            get
            {
                return ((this._generalPurposeBitFlag & 1) == 1);
            }
        }

        internal string FileName
        {
            get
            {
                return this._stringFileName;
            }
        }

        internal static int FixedMinimalRecordSize
        {
            get
            {
                return 30;
            }
        }

        internal ushort GeneralPurposeBitFlag
        {
            get
            {
                return this._generalPurposeBitFlag;
            }
        }

        internal uint LastModFileDateTime
        {
            get
            {
                return this._lastModFileDateTime;
            }
            set
            {
                this._lastModFileDateTime = value;
            }
        }

        internal long Size
        {
            get
            {
                return (long)((30 + this._fileNameLength) + this._extraField.Size);
            }
        }

        internal bool StreamingCreationFlag
        {
            get
            {
                return ((this._generalPurposeBitFlag & 8) != 0);
            }
            set
            {
                if (value)
                {
                    this._generalPurposeBitFlag = (ushort)(this._generalPurposeBitFlag | 8);
                }
                else
                {
                    this._generalPurposeBitFlag = (ushort)(this._generalPurposeBitFlag & 0xfff7);
                }
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
    }

}
