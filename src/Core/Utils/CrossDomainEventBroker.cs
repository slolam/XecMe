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
