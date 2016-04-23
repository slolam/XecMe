using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XecMe.Core.Diagnostics
{
    public delegate void Write(string log);
    public static class Log
    {
        static Log()
        {
            ErrorSink =  log => Trace.TraceError(log);
            InformationSink = log => Trace.TraceInformation(log);
            WarningSink = log => Trace.TraceWarning(log);
        }
        public static void Error(string log)
        {
            if (ErrorSink != null)
                ErrorSink(log);
        }
        public static void Information(string log)
        {
            if (InformationSink != null)
                InformationSink(log);
        }
        public static void Warning(string log)
        {
            if (WarningSink != null)
                WarningSink(log);
        }

        public static Write ErrorSink;
        public static Write InformationSink;
        public static Write WarningSink;
    }
}
