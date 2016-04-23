using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Axp.Fx.Common.Zip
{
    internal class ZipIOExtraField
    {
        // Fields
        private ArrayList _extraFieldElements;
        private ZipIOExtraFieldPaddingElement _paddingElement;
        private ZipIOExtraFieldZip64Element _zip64Element;

        // Methods
        private ZipIOExtraField()
        {
        }

        internal static ZipIOExtraField CreateNew(bool createPadding)
        {
            ZipIOExtraField field = new ZipIOExtraField();
            field._zip64Element = ZipIOExtraFieldZip64Element.CreateNew();
            if (createPadding)
            {
                field._paddingElement = ZipIOExtraFieldPaddingElement.CreateNew();
            }
            return field;
        }

        internal static ZipIOExtraField ParseRecord(BinaryReader reader, ZipIOZip64ExtraFieldUsage zip64extraFieldUsage, ushort expectedExtraFieldSize)
        {
            if (expectedExtraFieldSize == 0)
            {
                if (zip64extraFieldUsage != ZipIOZip64ExtraFieldUsage.None)
                {
                    throw new FileFormatException(SR.Get("CorruptedData"));
                }
                return CreateNew(false);
            }
            ZipIOExtraField field = new ZipIOExtraField();
            while (expectedExtraFieldSize > 0)
            {
                if (expectedExtraFieldSize < ZipIOExtraFieldElement.MinimumSize)
                {
                    throw new FileFormatException(SR.Get("CorruptedData"));
                }
                ZipIOExtraFieldElement element = ZipIOExtraFieldElement.Parse(reader, zip64extraFieldUsage);
                ZipIOExtraFieldZip64Element element3 = element as ZipIOExtraFieldZip64Element;
                ZipIOExtraFieldPaddingElement element2 = element as ZipIOExtraFieldPaddingElement;
                if (element3 != null)
                {
                    if (field._zip64Element != null)
                    {
                        throw new FileFormatException(SR.Get("CorruptedData"));
                    }
                    field._zip64Element = element3;
                }
                else if (element2 != null)
                {
                    if (field._paddingElement != null)
                    {
                        throw new FileFormatException(SR.Get("CorruptedData"));
                    }
                    field._paddingElement = element2;
                }
                else
                {
                    if (field._extraFieldElements == null)
                    {
                        field._extraFieldElements = new ArrayList(3);
                    }
                    field._extraFieldElements.Add(element);
                }
                expectedExtraFieldSize = (ushort)(expectedExtraFieldSize - element.Size);
            }
            if (expectedExtraFieldSize != 0)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            if (field._zip64Element == null)
            {
                field._zip64Element = ZipIOExtraFieldZip64Element.CreateNew();
            }
            return field;
        }

        internal void Save(BinaryWriter writer)
        {
            if (this._zip64Element.SizeField > 0)
            {
                this._zip64Element.Save(writer);
            }
            if (this._paddingElement != null)
            {
                this._paddingElement.Save(writer);
            }
            if (this._extraFieldElements != null)
            {
                foreach (ZipIOExtraFieldElement element in this._extraFieldElements)
                {
                    element.Save(writer);
                }
            }
        }

        internal void UpdatePadding(long size)
        {
            if (Math.Abs(size) <= 0xffffL)
            {
                if ((size > 0L) && (this._paddingElement != null))
                {
                    if (this._paddingElement.PaddingSize >= size)
                    {
                        this._paddingElement.PaddingSize = (ushort)(this._paddingElement.PaddingSize - ((ushort)size));
                    }
                    else if (this._paddingElement.Size == size)
                    {
                        this._paddingElement = null;
                    }
                }
                else if (size < 0L)
                {
                    if (this._paddingElement == null)
                    {
                        size += ZipIOExtraFieldPaddingElement.MinimumFieldDataSize + ZipIOExtraFieldElement.MinimumSize;
                        if (size >= 0L)
                        {
                            this._paddingElement = new ZipIOExtraFieldPaddingElement();
                            this._paddingElement.PaddingSize = (ushort)size;
                        }
                    }
                    else if ((this._paddingElement.PaddingSize - size) <= 0xffffL)
                    {
                        this._paddingElement.PaddingSize = (ushort)(this._paddingElement.PaddingSize - size);
                    }
                }
            }
        }

        // Properties
        internal long CompressedSize
        {
            get
            {
                return this._zip64Element.CompressedSize;
            }
            set
            {
                this._zip64Element.CompressedSize = value;
            }
        }

        internal uint DiskNumberOfFileStart
        {
            get
            {
                return this._zip64Element.DiskNumber;
            }
        }

        internal long OffsetOfLocalHeader
        {
            get
            {
                return this._zip64Element.OffsetOfLocalHeader;
            }
            set
            {
                this._zip64Element.OffsetOfLocalHeader = value;
            }
        }

        internal ushort Size
        {
            get
            {
                ushort num = 0;
                if (this._extraFieldElements != null)
                {
                    foreach (ZipIOExtraFieldElement element in this._extraFieldElements)
                    {
                        num = (ushort)(num + element.Size);
                    }
                }
                num = (ushort)(num + this._zip64Element.Size);
                if (this._paddingElement != null)
                {
                    num = (ushort)(num + this._paddingElement.Size);
                }
                return num;
            }
        }

        internal long UncompressedSize
        {
            get
            {
                return this._zip64Element.UncompressedSize;
            }
            set
            {
                this._zip64Element.UncompressedSize = value;
            }
        }

        internal ZipIOZip64ExtraFieldUsage Zip64ExtraFieldUsage
        {
            get
            {
                return this._zip64Element.Zip64ExtraFieldUsage;
            }
            set
            {
                this._zip64Element.Zip64ExtraFieldUsage = value;
            }
        }
    }

    internal class ZipIOExtraFieldElement
    {
        // Fields
        private byte[] _data;
        private ushort _id;
        private static readonly ushort _minimumSize = 4;
        private ushort _size;

        // Methods
        internal ZipIOExtraFieldElement(ushort id)
        {
            this._id = id;
        }

        private ZipIOExtraFieldElement(ushort id, byte[] data)
        {
            this._id = id;
            this._data = data;
            this._size = (ushort)data.Length;
        }

        internal static ZipIOExtraFieldElement Parse(BinaryReader reader, ZipIOZip64ExtraFieldUsage zip64extraFieldUsage)
        {
            ZipIOExtraFieldElement element;
            ushort id = reader.ReadUInt16();
            ushort size = reader.ReadUInt16();
            if (id == ZipIOExtraFieldZip64Element.ConstantFieldId)
            {
                element = new ZipIOExtraFieldZip64Element();
                ((ZipIOExtraFieldZip64Element)element).Zip64ExtraFieldUsage = zip64extraFieldUsage;
            }
            else if (id == ZipIOExtraFieldPaddingElement.ConstantFieldId)
            {
                if (size < ZipIOExtraFieldPaddingElement.MinimumFieldDataSize)
                {
                    element = new ZipIOExtraFieldElement(id);
                }
                else
                {
                    byte[] sniffiedBytes = reader.ReadBytes(ZipIOExtraFieldPaddingElement.SignatureSize);
                    if (ZipIOExtraFieldPaddingElement.MatchesPaddingSignature(sniffiedBytes))
                    {
                        element = new ZipIOExtraFieldPaddingElement();
                    }
                    else
                    {
                        element = new ZipIOExtraFieldElement(id, sniffiedBytes);
                    }
                }
            }
            else
            {
                element = new ZipIOExtraFieldElement(id);
            }
            element.ParseDataField(reader, size);
            return element;
        }

        internal virtual void ParseDataField(BinaryReader reader, ushort size)
        {
            if (this._data == null)
            {
                this._data = reader.ReadBytes(size);
                if (this._data.Length != size)
                {
                    throw new FileFormatException(SR.Get("CorruptedData"));
                }
            }
            else
            {
                byte[] sourceArray = this._data;
                this._data = new byte[size];
                Array.Copy(sourceArray, this._data, (int)this._size);
                if ((PackagingUtilities.ReliableRead(reader, this._data, this._size, size - this._size) + this._size) != size)
                {
                    throw new FileFormatException(SR.Get("CorruptedData"));
                }
            }
            this._size = size;
        }

        internal virtual void Save(BinaryWriter writer)
        {
            writer.Write(this._id);
            writer.Write(this._size);
            writer.Write(this._data);
        }

        // Properties
        internal virtual byte[] DataField
        {
            get
            {
                return this._data;
            }
        }

        internal static ushort MinimumSize
        {
            get
            {
                return _minimumSize;
            }
        }

        internal virtual ushort Size
        {
            get
            {
                return (ushort)(this._size + _minimumSize);
            }
        }

        internal virtual ushort SizeField
        {
            get
            {
                return this._size;
            }
        }
    }

    internal class ZipIOExtraFieldPaddingElement : ZipIOExtraFieldElement
    {
        // Fields
        private const ushort _constantFieldId = 0xa220;
        private ushort _initialRequestedPaddingSize;
        private static readonly ushort _minimumFieldDataSize = 4;
        private const ushort _newInitialPaddingSize = 20;
        private ushort _paddingSize;
        private const ushort _signature = 0xa028;
        private static readonly ushort _signatureSize = 2;

        // Methods
        internal ZipIOExtraFieldPaddingElement()
            : base(0xa220)
        {
            this._initialRequestedPaddingSize = 20;
            this._paddingSize = this._initialRequestedPaddingSize;
        }

        internal static ZipIOExtraFieldPaddingElement CreateNew()
        {
            return new ZipIOExtraFieldPaddingElement();
        }

        internal static bool MatchesPaddingSignature(byte[] sniffiedBytes)
        {
            if (sniffiedBytes.Length < _signatureSize)
            {
                return false;
            }
            if (BitConverter.ToUInt16(sniffiedBytes, 0) != 0xa028)
            {
                return false;
            }
            return true;
        }

        internal override void ParseDataField(BinaryReader reader, ushort size)
        {
            this._initialRequestedPaddingSize = reader.ReadUInt16();
            size = (ushort)(size - _minimumFieldDataSize);
            this._paddingSize = size;
            if (this._paddingSize != 0)
            {
                reader.BaseStream.Seek((long)size, SeekOrigin.Current);
            }
        }

        internal override void Save(BinaryWriter writer)
        {
            writer.Write((ushort)0xa220);
            writer.Write(this.SizeField);
            writer.Write((ushort)0xa028);
            writer.Write(this._initialRequestedPaddingSize);
            for (int i = 0; i < this._paddingSize; i++)
            {
                writer.Write((byte)0);
            }
        }

        // Properties
        internal static ushort ConstantFieldId
        {
            get
            {
                return 0xa220;
            }
        }

        internal static ushort MinimumFieldDataSize
        {
            get
            {
                return _minimumFieldDataSize;
            }
        }

        internal ushort PaddingSize
        {
            get
            {
                return this._paddingSize;
            }
            set
            {
                this._paddingSize = value;
            }
        }

        internal static ushort SignatureSize
        {
            get
            {
                return _signatureSize;
            }
        }

        internal override ushort Size
        {
            get
            {
                return (ushort)(this.SizeField + ZipIOExtraFieldElement.MinimumSize);
            }
        }

        internal override ushort SizeField
        {
            get
            {
                return (ushort)(_minimumFieldDataSize + this._paddingSize);
            }
        }
    }

    internal class ZipIOExtraFieldZip64Element : ZipIOExtraFieldElement
    {
        // Fields
        private ulong _compressedSize;
        private const ushort _constantFieldId = 1;
        private uint _diskNumber;
        private ulong _offsetOfLocalHeader;
        private ulong _uncompressedSize;
        private ZipIOZip64ExtraFieldUsage _zip64ExtraFieldUsage;

        // Methods
        internal ZipIOExtraFieldZip64Element()
            : base(1)
        {
            this._zip64ExtraFieldUsage = ZipIOZip64ExtraFieldUsage.None;
        }

        internal static ZipIOExtraFieldZip64Element CreateNew()
        {
            return new ZipIOExtraFieldZip64Element();
        }

        internal override void ParseDataField(BinaryReader reader, ushort size)
        {
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.UncompressedSize) != ZipIOZip64ExtraFieldUsage.None)
            {
                this._uncompressedSize = reader.ReadUInt64();
                if (size < 8)
                {
                    throw new FileFormatException(SR.Get("CorruptedData"));
                }
                size = (ushort)(size - 8);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.CompressedSize) != ZipIOZip64ExtraFieldUsage.None)
            {
                this._compressedSize = reader.ReadUInt64();
                if (size < 8)
                {
                    throw new FileFormatException(SR.Get("CorruptedData"));
                }
                size = (ushort)(size - 8);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader) != ZipIOZip64ExtraFieldUsage.None)
            {
                this._offsetOfLocalHeader = reader.ReadUInt64();
                if (size < 8)
                {
                    throw new FileFormatException(SR.Get("CorruptedData"));
                }
                size = (ushort)(size - 8);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.DiskNumber) != ZipIOZip64ExtraFieldUsage.None)
            {
                this._diskNumber = reader.ReadUInt32();
                if (size < 4)
                {
                    throw new FileFormatException(SR.Get("CorruptedData"));
                }
                size = (ushort)(size - 4);
            }
            if (size != 0)
            {
                throw new FileFormatException(SR.Get("CorruptedData"));
            }
            this.Validate();
        }

        internal override void Save(BinaryWriter writer)
        {
            writer.Write((ushort)1);
            writer.Write(this.SizeField);
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.UncompressedSize) != ZipIOZip64ExtraFieldUsage.None)
            {
                writer.Write(this._uncompressedSize);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.CompressedSize) != ZipIOZip64ExtraFieldUsage.None)
            {
                writer.Write(this._compressedSize);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader) != ZipIOZip64ExtraFieldUsage.None)
            {
                writer.Write(this._offsetOfLocalHeader);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.DiskNumber) != ZipIOZip64ExtraFieldUsage.None)
            {
                writer.Write(this._diskNumber);
            }
        }

        private void Validate()
        {
            if (((this._compressedSize >= 0x7fffffffffffffffL) || (this._uncompressedSize >= 0x7fffffffffffffffL)) || (this._offsetOfLocalHeader >= 0x7fffffffffffffffL))
            {
                throw new NotSupportedException(SR.Get("Zip64StructuresTooLarge"));
            }
            if (this._diskNumber != 0)
            {
                throw new NotSupportedException(SR.Get("NotSupportedMultiDisk"));
            }
        }

        // Properties
        internal long CompressedSize
        {
            get
            {
                return (long)this._compressedSize;
            }
            set
            {
                this._zip64ExtraFieldUsage |= ZipIOZip64ExtraFieldUsage.CompressedSize;
                this._compressedSize = (ulong)value;
            }
        }

        internal static ushort ConstantFieldId
        {
            get
            {
                return 1;
            }
        }

        internal uint DiskNumber
        {
            get
            {
                return this._diskNumber;
            }
        }

        internal long OffsetOfLocalHeader
        {
            get
            {
                return (long)this._offsetOfLocalHeader;
            }
            set
            {
                this._zip64ExtraFieldUsage |= ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader;
                this._offsetOfLocalHeader = (ulong)value;
            }
        }

        internal override ushort Size
        {
            get
            {
                if (this.SizeField == 0)
                {
                    return 0;
                }
                return (ushort)(this.SizeField + ZipIOExtraFieldElement.MinimumSize);
            }
        }

        internal override ushort SizeField
        {
            get
            {
                ushort num = 0;
                if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.UncompressedSize) != ZipIOZip64ExtraFieldUsage.None)
                {
                    num = (ushort)(num + 8);
                }
                if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.CompressedSize) != ZipIOZip64ExtraFieldUsage.None)
                {
                    num = (ushort)(num + 8);
                }
                if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader) != ZipIOZip64ExtraFieldUsage.None)
                {
                    num = (ushort)(num + 8);
                }
                if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.DiskNumber) != ZipIOZip64ExtraFieldUsage.None)
                {
                    num = (ushort)(num + 4);
                }
                return num;
            }
        }

        internal long UncompressedSize
        {
            get
            {
                return (long)this._uncompressedSize;
            }
            set
            {
                this._zip64ExtraFieldUsage |= ZipIOZip64ExtraFieldUsage.UncompressedSize;
                this._uncompressedSize = (ulong)value;
            }
        }

        internal ZipIOZip64ExtraFieldUsage Zip64ExtraFieldUsage
        {
            get
            {
                return this._zip64ExtraFieldUsage;
            }
            set
            {
                this._zip64ExtraFieldUsage = value;
            }
        }
    }

}
