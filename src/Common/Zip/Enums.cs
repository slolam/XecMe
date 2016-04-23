using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Axp.Fx.Common.Zip
{
    internal enum ZipIOZip64ExtraFieldUsage
    {
        CompressedSize = 2,
        DiskNumber = 8,
        None = 0,
        OffsetOfLocalHeader = 4,
        UncompressedSize = 1
    }

    internal enum ZipIOVersionNeededToExtract : ushort
    {
        DeflatedData = 20,
        StoredData = 10,
        VolumeLabel = 11,
        Zip64FileFormat = 0x2d
    }

    internal enum PreSaveNotificationScanControlInstruction
    {
        Continue,
        Stop
    }




    internal enum DeflateOptionEnum : byte
    {
        Fast = 4,
        Maximum = 2,
        None = 0xff,
        Normal = 0,
        SuperFast = 6
    }




    internal enum CompressionMethodEnum : ushort
    {
        Deflated = 8,
        Stored = 0
    }

 

 
 

 

}
