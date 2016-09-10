#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file Arguments.cs is part of XecMe.
/// 
/// XecMe is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
/// 
/// XecMe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with XecMe. If not, see http://www.gnu.org/licenses/.
/// 
/// History:
/// ______________________________________________________________
/// Created         06-2014             Shailesh Lolam

#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XecMe.Common.Diagnostics
{
    /// <summary>
    /// Delegate to hook up to any logging framework like log4net to funnel the logging into single log repository
    /// </summary>
    /// <param name="log"></param>
    public delegate void Write(string log);

    /// <summary>
    /// Use this class to log the message or hook this class to any logging framework using the delegates on this. By default it hooks up to standard .NET Trace
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Initializes the <see cref="Log"/> class.
        /// </summary>
        static Log()
        {
            ErrorSink =  log => Trace.TraceError(log);
            InformationSink = log => Trace.TraceInformation(log);
            WarningSink = log => Trace.TraceWarning(log);
        }
        /// <summary>
        /// Logs the message as error by calling ErrorSink delegate that by default logs to Trace.TraceError
        /// </summary>
        /// <param name="log">Error message to be logged</param>
        public static void Error(string log)
        {
            if (ErrorSink != null)
                ErrorSink(log);
        }
        /// <summary>
        /// Logs the message as information by calling InformatioSink delegate that by default logs to Trace.TraceInformation
        /// </summary>
        /// <param name="log">Information message to be logged</param>
        public static void Information(string log)
        {
            if (InformationSink != null)
                InformationSink(log);
        }

        /// <summary>
        /// Logs the message as warning by calling WarningSink delegate that by default logs to Trace.TraceWarning
        /// </summary>
        /// <param name="log">Wanring message to be logged</param>
        public static void Warning(string log)
        {
            if (WarningSink != null)
                WarningSink(log);
        }

        /// <summary>
        /// Delegate to sink error messages, default it's hooked to Trace.TraceError
        /// </summary>
        public static Write ErrorSink;
        /// <summary>
        /// Delegate to sink error messages, default it's hooked to Trace.TraceInformation
        /// </summary>
        public static Write InformationSink;
        /// <summary>
        /// Delegate to sink error messages, default it's hooked to Trace.TraceWarning
        /// </summary>
        public static Write WarningSink;
    }
}
