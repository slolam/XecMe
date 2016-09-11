using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIOBlockManager : IDisposable, IEnumerable
    {
        // Fields
        private Stream _archiveStream;
        private BinaryReader _binaryReader;
        private BinaryWriter _binaryWriter;
        private ArrayList _blockList = new ArrayList(50);
        private ZipIOCentralDirectoryBlock _centralDirectoryBlock;
        private bool _dirtyFlag;
        private bool _disposedFlag;
        private ASCIIEncoding _encoding = new ASCIIEncoding();
        private ZipIOEndOfCentralDirectoryBlock _endOfCentralDirectoryBlock;
        private const long _highWaterMark = 0xa00000L;
        private const int _initialBlockListSize = 50;
        private const long _lowWaterMark = 0x19000L;
        private bool _openStreaming;
        private bool _ownStream;
        private bool _propagatingFlushDisposed;
        private const int _requiredBlockCount = 4;
        private ZipIOLocalFileBlock _streamingCurrentlyOpenStreamBlock;
        private ZipIOZip64EndOfCentralDirectoryBlock _zip64EndOfCentralDirectoryBlock;
        private ZipIOZip64EndOfCentralDirectoryLocatorBlock _zip64EndOfCentralDirectoryLocatorBlock;

        // Methods
        internal ZipIOBlockManager(Stream archiveStream, bool streaming, bool ownStream)
        {
            this._archiveStream = archiveStream;
            this._openStreaming = streaming;
            this._ownStream = ownStream;
            if (streaming)
            {
                this._archiveStream = new WriteTimeStream(this._archiveStream);
            }
            else if (archiveStream.Length > 0L)
            {
                ZipIORawDataFileBlock block = ZipIORawDataFileBlock.Assign(this, 0L, archiveStream.Length);
                this._blockList.Add(block);
            }
        }

        private void AppendBlock(IZipIOBlock block)
        {
            this._blockList.Add(block);
        }

        internal static ZipIOVersionNeededToExtract CalcVersionNeededToExtractFromCompression(CompressionMethodEnum compression)
        {
            CompressionMethodEnum enum2 = compression;
            if (enum2 != CompressionMethodEnum.Stored)
            {
                if (enum2 != CompressionMethodEnum.Deflated)
                {
                    throw new NotSupportedException();
                }
                return ZipIOVersionNeededToExtract.DeflatedData;
            }
            return ZipIOVersionNeededToExtract.StoredData;
        }

        private void CheckDisposed()
        {
            if (this._disposedFlag)
            {
                throw new ObjectDisposedException(null, SR.Get("ZipArchiveDisposed"));
            }
        }

        internal static PreSaveNotificationScanControlInstruction CommonPreSaveNotificationHandler(Stream stream, long offset, long size, long onDiskOffset, long onDiskSize, ref SparseMemoryStream cachePrefixStream)
        {
            if (size != 0L)
            {
                long num2;
                long num4;
                if (cachePrefixStream != null)
                {
                    onDiskOffset += cachePrefixStream.Length;
                    onDiskSize -= cachePrefixStream.Length;
                }
                if (onDiskSize == 0L)
                {
                    return PreSaveNotificationScanControlInstruction.Continue;
                }
                PackagingUtilities.CalculateOverlap(onDiskOffset, onDiskSize, offset, size, out num4, out num2);
                if (num2 <= 0L)
                {
                    if (onDiskOffset <= offset)
                    {
                        return PreSaveNotificationScanControlInstruction.Continue;
                    }
                    return PreSaveNotificationScanControlInstruction.Stop;
                }
                long bytesToCopy = (num4 + num2) - onDiskOffset;
                if (cachePrefixStream == null)
                {
                    cachePrefixStream = new SparseMemoryStream(0x19000L, 0xa00000L);
                }
                else
                {
                    cachePrefixStream.Seek(0L, SeekOrigin.End);
                }
                stream.Seek(onDiskOffset, SeekOrigin.Begin);
                
                if (PackagingUtilities.CopyStream(stream, cachePrefixStream, bytesToCopy, 0x1000) != bytesToCopy)
                {
                    throw new FileFormatException(SR.Get("CorruptedData"));
                }
                if ((onDiskOffset + onDiskSize) < (offset + size))
                {
                    return PreSaveNotificationScanControlInstruction.Continue;
                }
            }
            return PreSaveNotificationScanControlInstruction.Stop;
        }

        internal static ulong ConvertToUInt64(uint loverAddressValue, uint higherAddressValue)
        {
            return (loverAddressValue + (higherAddressValue << 0x20));
        }

        internal static int CopyBytes(short value, byte[] buffer, int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
            return (offset + bytes.Length);
        }

        internal static int CopyBytes(int value, byte[] buffer, int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
            return (offset + bytes.Length);
        }

        internal static int CopyBytes(long value, byte[] buffer, int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
            return (offset + bytes.Length);
        }

        internal static int CopyBytes(ushort value, byte[] buffer, int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
            return (offset + bytes.Length);
        }

        internal static int CopyBytes(uint value, byte[] buffer, int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
            return (offset + bytes.Length);
        }

        internal static int CopyBytes(ulong value, byte[] buffer, int offset)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
            return (offset + bytes.Length);
        }

        private void CreateCentralDirectoryBlock()
        {
            this.CheckDisposed();
            int index = this._blockList.IndexOf(this.Zip64EndOfCentralDirectoryBlock);
            this._centralDirectoryBlock = ZipIOCentralDirectoryBlock.CreateNew(this);
            this.InsertBlock(index, this._centralDirectoryBlock);
        }

        internal void CreateEndOfCentralDirectoryBlock()
        {
            this.CheckDisposed();
            long offset = 0L;
            this._endOfCentralDirectoryBlock = ZipIOEndOfCentralDirectoryBlock.CreateNew(this, offset);
            this.AppendBlock(this._endOfCentralDirectoryBlock);
            this.DirtyFlag = true;
        }

        private void CreateLoadZip64Blocks()
        {
            this.CheckDisposed();
            if ((!this.Streaming && this.EndOfCentralDirectoryBlock.ContainValuesHintingToPossibilityOfZip64) && ZipIOZip64EndOfCentralDirectoryLocatorBlock.SniffTheBlockSignature(this))
            {
                this.LoadZip64EndOfCentralDirectoryLocatorBlock();
                this.LoadZip64EndOfCentralDirectoryBlock();
            }
            else
            {
                this._endOfCentralDirectoryBlock.ValidateZip64TriggerValues();
                this.CreateZip64EndOfCentralDirectoryLocatorBlock();
                this.CreateZip64EndOfCentralDirectoryBlock();
            }
        }

        internal ZipIOLocalFileBlock CreateLocalFileBlock(string zipFileName, CompressionMethodEnum compressionMethod, DeflateOptionEnum deflateOption)
        {
            this.CheckDisposed();
            ZipIOLocalFileBlock block = ZipIOLocalFileBlock.CreateNew(this, zipFileName, compressionMethod, deflateOption);
            this.InsertBlock(this.CentralDirectoryBlockIndex, block);
            this.CentralDirectoryBlock.AddFileBlock(block);
            this.DirtyFlag = true;
            return block;
        }

        private void CreateZip64EndOfCentralDirectoryBlock()
        {
            int index = this._blockList.IndexOf(this.Zip64EndOfCentralDirectoryLocatorBlock);
            this._zip64EndOfCentralDirectoryBlock = ZipIOZip64EndOfCentralDirectoryBlock.CreateNew(this);
            this.InsertBlock(index, this._zip64EndOfCentralDirectoryBlock);
        }

        private void CreateZip64EndOfCentralDirectoryLocatorBlock()
        {
            int index = this._blockList.IndexOf(this.EndOfCentralDirectoryBlock);
            this._zip64EndOfCentralDirectoryLocatorBlock = ZipIOZip64EndOfCentralDirectoryLocatorBlock.CreateNew(this);
            this.InsertBlock(index, this._zip64EndOfCentralDirectoryLocatorBlock);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if ((disposing && !this._disposedFlag) && !this._propagatingFlushDisposed)
            {
                this._propagatingFlushDisposed = true;
                try
                {
                    try
                    {
                        foreach (IZipIOBlock block in this._blockList)
                        {
                            IDisposable disposable2 = block as IDisposable;
                            if (disposable2 != null)
                            {
                                disposable2.Dispose();
                            }
                        }
                    }
                    finally
                    {
                        if (this._ownStream)
                        {
                            if (this._binaryReader != null)
                            {
                                this._binaryReader.Close();
                            }
                            if (this._binaryWriter != null)
                            {
                                this._binaryWriter.Close();
                            }
                            this._archiveStream.Close();
                        }
                    }
                }
                finally
                {
                    this._blockList = null;
                    this._encoding = null;
                    this._endOfCentralDirectoryBlock = null;
                    this._centralDirectoryBlock = null;
                    this._disposedFlag = true;
                    this._propagatingFlushDisposed = false;
                }
            }
        }

        internal static DateTime FromMsDosDateTime(uint dosDateTime)
        {
            int second = (int)((dosDateTime & 0x1f) << 1);
            int minute = ((int)(dosDateTime >> 5)) & 0x3f;
            int hour = ((int)(dosDateTime >> 11)) & 0x1f;
            int day = ((int)(dosDateTime >> 0x10)) & 0x1f;
            int month = ((int)(dosDateTime >> 0x15)) & 15;
            return new DateTime(0x7bc + (((int)(dosDateTime >> 0x19)) & 0x7f), month, day, hour, minute, second);
        }

        private void InsertBlock(int blockPosition, IZipIOBlock block)
        {
            this._blockList.Insert(blockPosition, block);
        }

        private void LoadCentralDirectoryBlock()
        {
            this._centralDirectoryBlock = ZipIOCentralDirectoryBlock.SeekableLoad(this);
            this.MapBlock(this._centralDirectoryBlock);
        }

        internal void LoadEndOfCentralDirectoryBlock()
        {
            this._endOfCentralDirectoryBlock = ZipIOEndOfCentralDirectoryBlock.SeekableLoad(this);
            this.MapBlock(this._endOfCentralDirectoryBlock);
        }

        internal ZipIOLocalFileBlock LoadLocalFileBlock(string zipFileName)
        {
            this.CheckDisposed();
            ZipIOLocalFileBlock block = ZipIOLocalFileBlock.SeekableLoad(this, zipFileName);
            this.MapBlock(block);
            return block;
        }

        private void LoadZip64EndOfCentralDirectoryBlock()
        {
            this._zip64EndOfCentralDirectoryBlock = ZipIOZip64EndOfCentralDirectoryBlock.SeekableLoad(this);
            this.MapBlock(this._zip64EndOfCentralDirectoryBlock);
        }

        private void LoadZip64EndOfCentralDirectoryLocatorBlock()
        {
            this._zip64EndOfCentralDirectoryLocatorBlock = ZipIOZip64EndOfCentralDirectoryLocatorBlock.SeekableLoad(this);
            this.MapBlock(this._zip64EndOfCentralDirectoryLocatorBlock);
        }

        private void MapBlock(IZipIOBlock block)
        {
            for (int i = this._blockList.Count - 1; i >= 0; i--)
            {
                ZipIORawDataFileBlock block2 = this._blockList[i] as ZipIORawDataFileBlock;
                if ((block2 != null) && block2.DiskImageContains(block))
                {
                    ZipIORawDataFileBlock block3;
                    ZipIORawDataFileBlock block4;
                    block2.SplitIntoPrefixSuffix(block, out block4, out block3);
                    this._blockList.RemoveAt(i);
                    if (block3 != null)
                    {
                        this._blockList.Insert(i, block3);
                    }
                    this._blockList.Insert(i, block);
                    if (block4 != null)
                    {
                        this._blockList.Insert(i, block4);
                    }
                    return;
                }
            }
            throw new FileFormatException(SR.Get("CorruptedData"));
        }

        internal void MoveData(long moveBlockSourceOffset, long moveBlockTargetOffset, long moveBlockSize)
        {
            if ((moveBlockSize != 0L) && (moveBlockSourceOffset != moveBlockTargetOffset))
            {
                int num2;
                byte[] buffer = new byte[Math.Min(moveBlockSize, 0x100000L)];
                for (long i = 0L; i < moveBlockSize; i += num2)
                {
                    long num3;
                    long num4;
                    num2 = (int)Math.Min((long)buffer.Length, moveBlockSize - i);
                    if (moveBlockSourceOffset > moveBlockTargetOffset)
                    {
                        num4 = moveBlockSourceOffset + i;
                        num3 = moveBlockTargetOffset + i;
                    }
                    else
                    {
                        num4 = ((moveBlockSourceOffset + moveBlockSize) - i) - num2;
                        num3 = ((moveBlockTargetOffset + moveBlockSize) - i) - num2;
                    }
                    this._archiveStream.Seek(num4, SeekOrigin.Begin);
                    if (PackagingUtilities.ReliableRead(this._archiveStream, buffer, 0, num2) != num2)
                    {
                        throw new FileFormatException(SR.Get("CorruptedData"));
                    }
                    this._archiveStream.Seek(num3, SeekOrigin.Begin);
                    this._archiveStream.Write(buffer, 0, num2);
                }
            }
        }

        internal void RemoveLocalFileBlock(ZipIOLocalFileBlock localFileBlock)
        {
            this.CheckDisposed();
            this._blockList.Remove(localFileBlock);
            this.CentralDirectoryBlock.RemoveFileBlock(localFileBlock.FileName);
            this.DirtyFlag = true;
            localFileBlock.Dispose();
        }

        internal void Save(bool closingFlag)
        {
            this.CheckDisposed();
            if (!this._propagatingFlushDisposed)
            {
                this._propagatingFlushDisposed = true;
                try
                {
                    if (this._openStreaming)
                    {
                        this.StreamingSaveContainer(closingFlag);
                    }
                    else
                    {
                        this.SaveContainer(closingFlag);
                    }
                }
                finally
                {
                    this._propagatingFlushDisposed = false;
                }
            }
        }

        private void SaveContainer(bool closingFlag)
        {
            this.CheckDisposed();
            if (closingFlag || this.DirtyFlag)
            {
                long num2 = 0L;
                foreach (IZipIOBlock block2 in this._blockList)
                {
                    block2.Move(num2 - block2.Offset);
                    block2.UpdateReferences(closingFlag);
                    num2 += block2.Size;
                }
                bool flag = false;
                int count = this._blockList.Count;
                for (int i = 0; i < count; i++)
                {
                    IZipIOBlock block = (IZipIOBlock)this._blockList[i];
                    if (block.GetDirtyFlag(closingFlag))
                    {
                        flag = true;
                        long offset = block.Offset;
                        long size = block.Size;
                        if (size > 0L)
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                if (((IZipIOBlock)this._blockList[j]).PreSaveNotification(offset, size) == PreSaveNotificationScanControlInstruction.Stop)
                                {
                                    break;
                                }
                            }
                        }
                        block.Save();
                    }
                }
                if (flag && (this.Stream.Length > num2))
                {
                    this.Stream.SetLength(num2);
                }
                this.Stream.Flush();
                this.DirtyFlag = false;
            }
        }

        internal void SaveStream(ZipIOLocalFileBlock blockRequestingFlush, bool closingFlag)
        {
            if (!this._propagatingFlushDisposed)
            {
                this._propagatingFlushDisposed = true;
                try
                {
                    if (this._openStreaming)
                    {
                        this.StreamingSaveStream(blockRequestingFlush, closingFlag);
                    }
                    else
                    {
                        this.SaveContainer(false);
                    }
                }
                finally
                {
                    this._propagatingFlushDisposed = false;
                }
            }
        }

        private void StreamingSaveContainer(bool closingFlag)
        {
            try
            {
                long num2 = 0L;
                for (int i = 0; i < this._blockList.Count; i++)
                {
                    IZipIOBlock block = (IZipIOBlock)this._blockList[i];
                    ZipIOLocalFileBlock block2 = block as ZipIOLocalFileBlock;
                    if (block2 == null)
                    {
                        if (closingFlag)
                        {
                            block.Move(num2 - block.Offset);
                            block.UpdateReferences(closingFlag);
                            if (block.GetDirtyFlag(closingFlag))
                            {
                                block.Save();
                            }
                        }
                    }
                    else if (block.GetDirtyFlag(closingFlag))
                    {
                        block2.SaveStreaming(closingFlag);
                    }
                    num2 += block.Size;
                }
                this.Stream.Flush();
            }
            finally
            {
                this._propagatingFlushDisposed = false;
            }
        }

        private void StreamingSaveStream(ZipIOLocalFileBlock blockRequestingFlush, bool closingFlag)
        {
            if (this._streamingCurrentlyOpenStreamBlock != blockRequestingFlush)
            {
                if (this._streamingCurrentlyOpenStreamBlock != null)
                {
                    this._streamingCurrentlyOpenStreamBlock.SaveStreaming(true);
                }
                this._streamingCurrentlyOpenStreamBlock = blockRequestingFlush;
            }
            this._streamingCurrentlyOpenStreamBlock.SaveStreaming(closingFlag);
            if (closingFlag)
            {
                this._streamingCurrentlyOpenStreamBlock = null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.CheckDisposed();
            return this._blockList.GetEnumerator();
        }

        internal static uint ToMsDosDateTime(DateTime dateTime)
        {
            uint num = 0;
            num |= (uint)((dateTime.Second / 2) & 0x1f);
            num |= (uint)((dateTime.Minute & 0x3f) << 5);
            num |= (uint)((dateTime.Hour & 0x1f) << 11);
            num |= (uint)((dateTime.Day & 0x1f) << 0x10);
            num |= (uint)((dateTime.Month & 15) << 0x15);
            return (num | ((uint)(((dateTime.Year - 0x7bc) & 0x7f) << 0x19)));
        }

        internal static string ValidateNormalizeFileName(string zipFileName)
        {
            if (zipFileName == null)
            {
                throw new ArgumentNullException("zipFileName");
            }
            if (zipFileName.Length > MaxFileNameSize)
            {
                throw new ArgumentOutOfRangeException("zipFileName");
            }
            zipFileName = zipFileName.Trim();
            if (zipFileName.Length < 1)
            {
                throw new ArgumentOutOfRangeException("zipFileName");
            }
            return zipFileName;
        }

        // Properties
        internal BinaryReader BinaryReader
        {
            get
            {
                this.CheckDisposed();
                if (this._binaryReader == null)
                {
                    this._binaryReader = new BinaryReader(this.Stream, this.Encoding);
                }
                return this._binaryReader;
            }
        }

        internal BinaryWriter BinaryWriter
        {
            get
            {
                this.CheckDisposed();
                if (this._binaryWriter == null)
                {
                    this._binaryWriter = new BinaryWriter(this.Stream, this.Encoding);
                }
                return this._binaryWriter;
            }
        }

        internal ZipIOCentralDirectoryBlock CentralDirectoryBlock
        {
            get
            {
                this.CheckDisposed();
                if (this._centralDirectoryBlock == null)
                {
                    if (this.Zip64EndOfCentralDirectoryBlock.TotalNumberOfEntriesInTheCentralDirectory > 0)
                    {
                        this.LoadCentralDirectoryBlock();
                    }
                    else
                    {
                        this.CreateCentralDirectoryBlock();
                    }
                }
                return this._centralDirectoryBlock;
            }
        }

        private int CentralDirectoryBlockIndex
        {
            get
            {
                Invariant.Assert(this._blockList.Count >= 4);
                return (this._blockList.Count - 4);
            }
        }

        internal bool DirtyFlag
        {
            get
            {
                this.CheckDisposed();
                return this._dirtyFlag;
            }
            set
            {
                this.CheckDisposed();
                this._dirtyFlag = value;
            }
        }

        internal Encoding Encoding
        {
            get
            {
                this.CheckDisposed();
                return this._encoding;
            }
        }

        internal ZipIOEndOfCentralDirectoryBlock EndOfCentralDirectoryBlock
        {
            get
            {
                this.CheckDisposed();
                if (this._endOfCentralDirectoryBlock == null)
                {
                    this.LoadEndOfCentralDirectoryBlock();
                }
                return this._endOfCentralDirectoryBlock;
            }
        }

        internal bool IsCentralDirectoryBlockLoaded
        {
            get
            {
                this.CheckDisposed();
                return (this._centralDirectoryBlock != null);
            }
        }

        internal static int MaxFileNameSize
        {
            get
            {
                return 0xffff;
            }
        }

        internal Stream Stream
        {
            get
            {
                this.CheckDisposed();
                return this._archiveStream;
            }
        }

        internal bool Streaming
        {
            get
            {
                this.CheckDisposed();
                return this._openStreaming;
            }
        }

        internal ZipIOZip64EndOfCentralDirectoryBlock Zip64EndOfCentralDirectoryBlock
        {
            get
            {
                this.CheckDisposed();
                if (this._zip64EndOfCentralDirectoryBlock == null)
                {
                    this.CreateLoadZip64Blocks();
                }
                return this._zip64EndOfCentralDirectoryBlock;
            }
        }

        internal ZipIOZip64EndOfCentralDirectoryLocatorBlock Zip64EndOfCentralDirectoryLocatorBlock
        {
            get
            {
                this.CheckDisposed();
                if (this._zip64EndOfCentralDirectoryLocatorBlock == null)
                {
                    this.CreateLoadZip64Blocks();
                }
                return this._zip64EndOfCentralDirectoryLocatorBlock;
            }
        }
    }



}
