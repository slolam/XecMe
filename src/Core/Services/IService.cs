#if !NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Text;

namespace XecMe.Core.Services
{
    /// <summary>
    /// IService is the interface to be implmented by a class to make itself hosted as a Windows Service
    /// </summary>
    public interface IService
    {
        string ServiceName { get; set; }
        /// <summary>
        /// Gets a value indicating whether the service can be paused and resumed.
        /// </summary>
        bool CanPauseAndContinue { get; }

        /// <summary>
        /// When implemented in a derived class, System.ServiceProcess.ServiceBase.OnContinue()
        /// runs when a Continue command is sent to the service by the Service Control
        /// Manager (SCM). Specifies actions to take when a service resumes normal functioning
        /// after being paused.
        /// </summary>
        void OnContinue();

        /// <summary>
        /// When implemented in a derived class, executes when a Pause command is sent
        /// to the service by the Service Control Manager (SCM). Specifies actions to
        /// take when a service pauses.
        /// </summary>
        void OnPause();

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent
        /// to the service by the Service Control Manager (SCM). Specifies actions to
        /// take when a service stops running.
        /// </summary>
        void OnStop();

        /// <summary>
        /// When implemented in a derived class, executes when the system is shutting
        /// down. Specifies what should occur immediately prior to the system shutting
        /// down.
        /// </summary>
        void OnShutdown();

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent
        /// to the service by the Service Control Manager (SCM) or when the operating
        /// system starts (for a service that starts automatically). Specifies actions
        /// to take when the service starts.
        /// </summary>
        void OnStart();
    }
}
#endif