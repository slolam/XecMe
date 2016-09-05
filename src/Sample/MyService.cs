using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XecMe.Core.Services;
using XecMe.Core.Utils;
using XecMe.Core.Tasks;
using XecMe.Core.Fluent;

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
            FlowConfiguration config = new FlowConfiguration();
            config.ScheduledTask<MyTask>("scheduled")
                .RunByDaysOfTheMonths(Months.All)
                .OnDays(1, 3, 5, 32)
                .RunsAt(TimeSpan.Parse("12:00"))
                .OfLocalTimeZone()
                .Add();
            TaskManager.Start( new TaskManagerConfig());
        }
    }
}
