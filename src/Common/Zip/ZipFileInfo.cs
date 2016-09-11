using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Axp.Fx.Common.Zip
{
    internal sealed class ZipFileInfo
    {
        // Fields
        private ZipIOLocalFileBlock _fileBlock;
        private ZipArchive _zipArchive;

        // Methods
        internal ZipFileInfo(ZipArchive zipArchive, ZipIOLocalFileBlock fileBlock)
        {
            this._fileBlock = fileBlock;
            this._zipArchive = zipArchive;
        }

        private void CheckDisposed()
        {
            this._fileBlock.CheckDisposed();
        }

        internal Stream GetStream(FileMode mode, FileAccess access)
        {
            this.CheckDisposed();
            return this._fileBlock.GetStream(mode, access);
        }

        // Properties
        internal CompressionMethodEnum CompressionMethod
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.CompressionMethod;
            }
        }

        internal DeflateOptionEnum DeflateOption
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.DeflateOption;
            }
        }

        internal bool FolderFlag
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.FolderFlag;
            }
        }

        internal DateTime LastModFileDateTime
        {
            get
            {
                this.CheckDisposed();
                return ZipIOBlockManager.FromMsDosDateTime(this._fileBlock.LastModFileDateTime);
            }
        }

        internal ZipIOLocalFileBlock LocalFileBlock
        {
            get
            {
                return this._fileBlock;
            }
        }

        internal string Name
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.FileName;
            }
        }

        internal bool VolumeLabelFlag
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.VolumeLabelFlag;
            }
        }

        internal ZipArchive ZipArchive
        {
            get
            {
                this.CheckDisposed();
                return this._zipArchive;
            }
        }
    }



    internal class ZipFileInfoCollection : IEnumerable
    {
        // Fields
        private ICollection _zipFileInfoCollection;

        // Methods
        internal ZipFileInfoCollection(ICollection zipFileInfoCollection)
        {
            this._zipFileInfoCollection = zipFileInfoCollection;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._zipFileInfoCollection.GetEnumerator();
        }
    }



}
