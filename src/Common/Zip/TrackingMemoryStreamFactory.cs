using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal class TrackingMemoryStreamFactory : ITrackingMemoryStreamFactory
    {
        // Fields
        private long _bufferedMemoryConsumption;

        // Methods
        public MemoryStream Create()
        {
            return new TrackingMemoryStream(this);
        }

        public MemoryStream Create(int capacity)
        {
            return new TrackingMemoryStream(this, capacity);
        }

        public void ReportMemoryUsageDelta(int delta)
        {
            this._bufferedMemoryConsumption += delta;
        }

        // Properties
        internal long CurrentMemoryConsumption
        {
            get
            {
                return this._bufferedMemoryConsumption;
            }
        }
    }


}
