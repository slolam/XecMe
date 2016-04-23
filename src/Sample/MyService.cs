using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XecMe.Core.Services;
using XecMe.Core.Utils;
using XecMe.Core.Tasks;

namespace Sample
{
    public class MyService : IService
    {
        private string _serviceName;
        string IService.ServiceName
        {
            get
            {
                return _serviceName;
            }
            set
            {
                _serviceName = value;
            }
            
        }

        bool IService.CanPauseAndContinue
        {
            get { return false; }
        }

        void IService.OnContinue()
        {
            throw new NotImplementedException();
        }

        void IService.OnPause()
        {
            throw new NotImplementedException();
        }

        void IService.OnStop()
        {
            Console.WriteLine("Service stopping");
        }

        void IService.OnShutdown()
        {
            Console.WriteLine("Service stopping");
        }

        void IService.OnStart()
        {
            Console.WriteLine("Service starting");
            TaskManager.Start( new TaskManagerConfig());
        }
    }
}
