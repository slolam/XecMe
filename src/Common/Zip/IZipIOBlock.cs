using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal interface IZipIOBlock
    {
        // Methods
        bool GetDirtyFlag(bool closingFlag);
        void Move(long shiftSize);
        PreSaveNotificationScanControlInstruction PreSaveNotification(long offset, long size);
        void Save();
        void UpdateReferences(bool closingFlag);

        // Properties
        long Offset { get; }
        long Size { get; }
    }

    internal interface ITrackingMemoryStreamFactory
    {
        // Methods
        MemoryStream Create();
        MemoryStream Create(int capacity);
        void ReportMemoryUsageDelta(int delta);
    }

 

 

 

}
