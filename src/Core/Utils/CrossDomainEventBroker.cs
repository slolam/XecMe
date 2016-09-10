#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file CrossDomainEventBroker.cs is part of XecMe.
/// 
/// XecMe is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
/// 
/// XecMe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with XecMe. If not, see http://www.gnu.org/licenses/.
/// 
/// History:
/// ______________________________________________________________
/// Created         01-2013             Shailesh Lolam

#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace XecMe.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class CrossDomainEventBroker 
    {
        /// <summary>
        /// Occurs when [unhandled exception].
        /// </summary>
        public event UnhandledExceptionEventHandler UnhandledException;
        /// <summary>
        /// Occurs when [domain unload].
        /// </summary>
        public event EventHandler DomainUnload;

        /// <summary>
        /// Unhandleds the exception sink.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        public void UnhandledExceptionSink(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException(sender, e);
        }

        /// <summary>
        /// Domains the unload sink.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void DomainUnloadSink(object sender, EventArgs e)
        {
            DomainUnload(sender, e);
        }

    }
}
