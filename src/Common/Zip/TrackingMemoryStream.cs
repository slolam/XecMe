using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Axp.Fx.Common.Zip
{
    internal sealed class TrackingMemoryStream : MemoryStream
    {
        // Fields
        private int _lastReportedHighWaterMark;
        private ITrackingMemoryStreamFactory _memoryStreamFactory;

        // Methods
        internal TrackingMemoryStream(ITrackingMemoryStreamFactory memoryStreamFactory)
        {
            this._memoryStreamFactory = memoryStreamFactory;
            this.ReportIfNeccessary();
        }

        internal TrackingMemoryStream(ITrackingMemoryStreamFactory memoryStreamFactory, int capacity)
            : base(capacity)
        {
            this._memoryStreamFactory = memoryStreamFactory;
            this.ReportIfNeccessary();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this._memoryStreamFactory != null))
                {
                    this.SetLength(0L);
                    this.Capacity = 0;
                    this.ReportIfNeccessary();
                    this._memoryStreamFactory = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num = base.Read(buffer, offset, count);
            this.ReportIfNeccessary();
            return num;
        }

        private void ReportIfNeccessary()
        {
            if (this.Capacity != this._lastReportedHighWaterMark)
            {
                this._memoryStreamFactory.ReportMemoryUsageDelta(this.Capacity - this._lastReportedHighWaterMark);
                this._lastReportedHighWaterMark = this.Capacity;
            }
        }

        public override void SetLength(long value)
        {
            base.SetLength(value);
            this.ReportIfNeccessary();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            this.ReportIfNeccessary();
        }
    }

}
