using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
  internal class MemoryStreamBlock : IComparable<MemoryStreamBlock>
{
    // Fields
    private long _offset;
    private MemoryStream _stream;

    // Methods
    internal MemoryStreamBlock(MemoryStream stream, long offset)
    {
        this._stream = stream;
        this._offset = offset;
    }

    int IComparable<MemoryStreamBlock>.CompareTo(MemoryStreamBlock other)
    {
        if (other == null)
        {
            return 1;
        }
        if (this._offset == other.Offset)
        {
            return 0;
        }
        if (this._offset > other.Offset)
        {
            if (this._offset < other.EndOffset)
            {
                return 0;
            }
            return 1;
        }
        if (other.Offset < this.EndOffset)
        {
            return 0;
        }
        return -1;
    }

    // Properties
    internal long EndOffset
    {
        get
        {
            return (this._offset + ((this._stream == null) ? 0L : this._stream.Length));
        }
    }

    internal long Offset
    {
        get
        {
            return this._offset;
        }
        set
        {
            this._offset = value;
        }
    }

    internal MemoryStream Stream
    {
        get
        {
            return this._stream;
        }
    }
}


}
