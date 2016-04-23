using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Globalization;

namespace Axp.Fx.Common.Zip
{
    internal sealed class ZipArchive : IDisposable
    {
        // Fields
        private ZipIOBlockManager _blockManager;
        private bool _disposedFlag;
        private FileAccess _openAccess;
        private FileMode _openMode;
        private static int[,] _validOpenParameters = new int[,] { { 2, 2, 0, 1 }, { 2, 2, 1, 1 }, { 2, 3, 0, 0 }, { 1, 2, 0, 1 }, { 1, 2, 1, 1 }, { 1, 3, 0, 0 }, { 3, 1, 0, 1 }, { 3, 1, 0, 0 }, { 3, 1, 1, 1 }, { 3, 1, 1, 0 }, { 3, 1, 2, 1 }, { 3, 1, 3, 1 }, { 3, 3, 0, 0 }, { 4, 3, 0, 0 } };
        private IDictionary _zipFileInfoDictionary;
        private const int _zipFileInfoDictionaryInitialSize = 50;

        // Methods
        private ZipArchive(Stream archiveStream, FileMode mode, FileAccess access, bool streaming, bool ownStream)
        {
            this._blockManager = new ZipIOBlockManager(archiveStream, streaming, ownStream);
            this._openMode = mode;
            this._openAccess = access;
            if (((this._openMode == FileMode.CreateNew) || (this._openMode == FileMode.Create)) || ((this._openMode == FileMode.OpenOrCreate) && (archiveStream.Length == 0L)))
            {
                this._blockManager.CreateEndOfCentralDirectoryBlock();
            }
            else
            {
                this._blockManager.LoadEndOfCentralDirectoryBlock();
            }
        }

        internal ZipFileInfo AddFile(string zipFileName, CompressionMethodEnum compressionMethod, DeflateOptionEnum deflateOption)
        {
            this.CheckDisposed();
            if (this._openAccess == FileAccess.Read)
            {
                throw new InvalidOperationException(SR.Get("CanNotWriteInReadOnlyMode"));
            }
            zipFileName = ZipIOBlockManager.ValidateNormalizeFileName(zipFileName);
            if ((compressionMethod != CompressionMethodEnum.Stored) && (compressionMethod != CompressionMethodEnum.Deflated))
            {
                throw new ArgumentOutOfRangeException("compressionMethod");
            }
            if ((deflateOption < DeflateOptionEnum.Normal) || ((deflateOption > DeflateOptionEnum.SuperFast) && (deflateOption != DeflateOptionEnum.None)))
            {
                throw new ArgumentOutOfRangeException("deflateOption");
            }
            if (this.FileExists(zipFileName))
            {
                throw new InvalidOperationException(SR.Get("AttemptedToCreateDuplicateFileName"));
            }
            ZipIOLocalFileBlock fileBlock = this._blockManager.CreateLocalFileBlock(zipFileName, compressionMethod, deflateOption);
            ZipFileInfo info = new ZipFileInfo(this, fileBlock);
            this.ZipFileInfoDictionary.Add(info.Name, info);
            return info;
        }

        private void CheckDisposed()
        {
            if (this._disposedFlag)
            {
                throw new ObjectDisposedException(null, SR.Get("ZipArchiveDisposed"));
            }
        }

        internal void Close()
        {
            this.Dispose();
        }

        internal void DeleteFile(string zipFileName)
        {
            this.CheckDisposed();
            if (this._openAccess == FileAccess.Read)
            {
                throw new InvalidOperationException(SR.Get("CanNotWriteInReadOnlyMode"));
            }
            zipFileName = ZipIOBlockManager.ValidateNormalizeFileName(zipFileName);
            if (this.FileExists(zipFileName))
            {
                ZipFileInfo file = this.GetFile(zipFileName);
                this.ZipFileInfoDictionary.Remove(zipFileName);
                this._blockManager.RemoveLocalFileBlock(file.LocalFileBlock);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !this._disposedFlag)
            {
                try
                {
                    if ((this._openAccess == FileAccess.ReadWrite) || (this._openAccess == FileAccess.Write))
                    {
                        this._blockManager.Save(true);
                    }
                    this._blockManager.Dispose();
                }
                finally
                {
                    this._disposedFlag = true;
                }
            }
        }

        internal bool FileExists(string zipFileName)
        {
            this.CheckDisposed();
            zipFileName = ZipIOBlockManager.ValidateNormalizeFileName(zipFileName);
            return this._blockManager.CentralDirectoryBlock.FileExists(zipFileName);
        }

        internal void Flush()
        {
            this.CheckDisposed();
            this._blockManager.Save(false);
        }

        internal ZipFileInfo GetFile(string zipFileName)
        {
            this.CheckDisposed();
            if (this._openAccess == FileAccess.Write)
            {
                throw new InvalidOperationException(SR.Get("CanNotReadInWriteOnlyMode"));
            }
            zipFileName = ZipIOBlockManager.ValidateNormalizeFileName(zipFileName);
            if (this.ZipFileInfoDictionary.Contains(zipFileName))
            {
                return (ZipFileInfo)this.ZipFileInfoDictionary[zipFileName];
            }
            if (!this.FileExists(zipFileName))
            {
                throw new InvalidOperationException(SR.Get("FileDoesNotExists"));
            }
            ZipIOLocalFileBlock fileBlock = this._blockManager.LoadLocalFileBlock(zipFileName);
            ZipFileInfo info = new ZipFileInfo(this, fileBlock);
            this.ZipFileInfoDictionary.Add(info.Name, info);
            return info;
        }

        internal ZipFileInfoCollection GetFiles()
        {
            this.CheckDisposed();
            if (this._openAccess == FileAccess.Write)
            {
                throw new InvalidOperationException(SR.Get("CanNotReadInWriteOnlyMode"));
            }
            foreach (string str in this._blockManager.CentralDirectoryBlock.GetFileNamesCollection())
            {
                this.GetFile(str);
            }
            return new ZipFileInfoCollection(this.ZipFileInfoDictionary.Values);
        }

        internal static ZipArchive OpenOnFile(string path, FileMode mode, FileAccess access, FileShare share, bool streaming)
        {
            if ((mode == FileMode.OpenOrCreate) || (mode == FileMode.Open))
            {
                FileInfo info = new FileInfo(path);
                if (info.Exists && (info.Length == 0L))
                {
                    throw new FileFormatException(SR.Get("ZipZeroSizeFileIsNotValidArchive"));
                }
            }
            ZipArchive archive = null;
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, mode, access, share, 0x1000, streaming);
                ValidateModeAccessShareStreaming(stream, mode, access, share, streaming);
                archive = new ZipArchive(stream, mode, access, streaming, true);
            }
            catch
            {
                if (stream != null)
                {
                    stream.Close();
                }
                throw;
            }
            return archive;
        }

        internal static ZipArchive OpenOnStream(Stream stream, FileMode mode, FileAccess access, bool streaming)
        {
            ValidateModeAccessShareStreaming(stream, mode, access, FileShare.None, streaming);
            if (stream.CanSeek)
            {
                bool flag = stream.Length == 0L;
                switch (mode)
                {
                    case FileMode.CreateNew:
                        if (!flag)
                        {
                            throw new IOException(SR.Get("CreateNewOnNonEmptyStream"));
                        }
                        break;

                    case FileMode.Create:
                        if (!flag)
                        {
                            stream.SetLength(0L);
                        }
                        break;

                    case FileMode.Open:
                        if (flag)
                        {
                            throw new FileFormatException(SR.Get("ZipZeroSizeFileIsNotValidArchive"));
                        }
                        break;
                }
            }
            return new ZipArchive(stream, mode, access, streaming, false);
        }

        private static void ValidateModeAccessShareStreaming(Stream stream, FileMode mode, FileAccess access, FileShare share, bool streaming)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            ValidateModeAccessShareValidEnums(mode, access, share);
            ValidateModeAccessShareSupportedEnums(mode, share);
            ValidateModeAccessStreamStreamingCombinations(stream, access, streaming);
            int num5 = Convert.ToInt32(mode, CultureInfo.InvariantCulture);
            int num4 = Convert.ToInt32(access, CultureInfo.InvariantCulture);
            int num3 = Convert.ToInt32(share, CultureInfo.InvariantCulture);
            int num2 = Convert.ToInt32(streaming, CultureInfo.InvariantCulture);
            for (int i = 0; i < _validOpenParameters.GetLength(0); i++)
            {
                if (((_validOpenParameters[i, 0] == num5) && (_validOpenParameters[i, 1] == num4)) && ((_validOpenParameters[i, 2] == num3) && (_validOpenParameters[i, 3] == num2)))
                {
                    return;
                }
            }
            throw new ArgumentException(SR.Get("UnsupportedCombinationOfModeAccessShareStreaming"));
        }

        private static void ValidateModeAccessShareSupportedEnums(FileMode mode, FileShare share)
        {
            if ((mode == FileMode.Append) || (mode == FileMode.Truncate))
            {
                throw new NotSupportedException(SR.Get("TruncateAppendModesNotSupported"));
            }
            if ((share != FileShare.Read) && (share != FileShare.None))
            {
                throw new NotSupportedException(SR.Get("OnlyFileShareReadAndFileShareNoneSupported"));
            }
        }

        private static void ValidateModeAccessShareValidEnums(FileMode mode, FileAccess access, FileShare share)
        {
            if ((((mode != FileMode.Append) && (mode != FileMode.Create)) && ((mode != FileMode.CreateNew) && (mode != FileMode.Open))) && ((mode != FileMode.OpenOrCreate) && (mode != FileMode.Truncate)))
            {
                throw new ArgumentOutOfRangeException("mode");
            }
            if (((access != FileAccess.Read) && (access != FileAccess.ReadWrite)) && (access != FileAccess.Write))
            {
                throw new ArgumentOutOfRangeException("access");
            }
            if ((((share != FileShare.Delete) && (share != FileShare.Inheritable)) && ((share != FileShare.None) && (share != FileShare.Read))) && ((share != FileShare.ReadWrite) && (share != FileShare.Write)))
            {
                throw new ArgumentOutOfRangeException("share");
            }
        }

        private static void ValidateModeAccessStreamStreamingCombinations(Stream stream, FileAccess access, bool streaming)
        {
            if (((access == FileAccess.Read) || (access == FileAccess.ReadWrite)) && !stream.CanRead)
            {
                throw new ArgumentException(SR.Get("CanNotReadDataFromStreamWhichDoesNotSupportReading"));
            }
            if (((access == FileAccess.Write) || (access == FileAccess.ReadWrite)) && !stream.CanWrite)
            {
                throw new ArgumentException(SR.Get("CanNotWriteDataToStreamWhichDoesNotSupportWriting"));
            }
            if (!streaming && !stream.CanSeek)
            {
                throw new ArgumentException(SR.Get("CanNotOperateOnStreamWhichDoesNotSupportSeeking"));
            }
        }

        internal static void VerifyVersionNeededToExtract(ushort version)
        {
            switch (version)
            {
                case 10:
                case 11:
                case 20:
                case 0x2d:
                    return;
            }
            throw new NotSupportedException(SR.Get("NotSupportedVersionNeededToExtract"));
        }

        // Properties
        internal FileAccess OpenAccess
        {
            get
            {
                this.CheckDisposed();
                return this._openAccess;
            }
        }

        private IDictionary ZipFileInfoDictionary
        {
            get
            {
                if (this._zipFileInfoDictionary == null)
                {
                    this._zipFileInfoDictionary = new Hashtable(50, StringComparer.Ordinal);
                }
                return this._zipFileInfoDictionary;
            }
        }
    }



}
