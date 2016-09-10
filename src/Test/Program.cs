using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common.Diagnostics;
using XecMe.Core.Events;
using XecMe.Core.Fluent;
using XecMe.Core.Tasks;

namespace XecMe.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.WarningSink = Console.WriteLine;
            Log.InformationSink = Console.WriteLine;
            Log.ErrorSink = Console.WriteLine;

            FlowConfiguration config = new FlowConfiguration();
            config.TimerTask<TestTask>("Parallel Task", LogType.All)
                .RunEvery(100)
                .RepeatFor(-1)
                .OnWeekdays(Weekdays.All)
                .DuringPeriod(DateTime.MinValue, DateTime.MaxValue)
                .DuringTimeOfDay(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("23:59:59"))
                .OfLocalTimeZone()
                .AddParameter("count",10)
                .Add();

            //TaskManager.Bootstrap = c => c.Verify();
            TaskManager.Start(config);
            var obj = new Program();

            EventManager.AddPublisher("Event", obj, "Fire");
            for (int i = 0; i < 50; i++)
            {
                obj.Fire(obj, new EventArgs<int>(i));
            }

            TaskManager.WaitTasksToComplete();
            Console.ReadKey();
        }

        event XecMe.Core.Events.EventHandler<int> Fire;
    }
}
