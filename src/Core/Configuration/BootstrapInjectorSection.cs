using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace XecMe.Core.Configuration
{
    public class BootstrapInjectorSection : ConfigurationSection
    {
        #region Constants
        internal const string TASK_RUNNERS = "taskRunners";
        internal const string BOOTSTRAP_INJECTOR = "bootstrapInjector";
        #endregion
    }
}
