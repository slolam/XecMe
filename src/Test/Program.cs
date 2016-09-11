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
            //task to run on ThankGiving
            config.ScheduledAsyncTask<TestAsyncTask>("ThanksGiving task", LogType.All)
                .RunByWeeksOfTheMonths(Months.November)
                .OnWeeks(Weeks.Last)
                .OnWeekdays(Weekdays.Thursday)
                .RunsAt(TimeSpan.Parse("02:00"))
                .OfLocalTimeZone()
                .AddParameter("count", 10)
                .Add();

            ////task to run on Christmas
            //config.ScheduledTask<TestAsyncTask>("Christmas Email blaster", LogType.All)
            //    .RunByDaysOfTheMonths(Months.December)
            //    .OnDays(25)
            //    .RunsAt(TimeSpan.Parse("02:00"))
            //    .OfLocalTimeZone()
            //    .AddParameter("count", 10)
            //    .Add();

            ////task to run every other day starting from Jan 01, 2016
            //config.ScheduledTask<TestAsyncTask>("task name", LogType.All)
            //    .RunDaily()
            //    .RepeatEvery(2) // every other day
            //    .StartsFrom(DateTime.Parse("01/01/2016")) //reference point to skip every other day
            //    .RunsAt(TimeSpan.Parse("06:00"))
            //    .OfLocalTimeZone()
            //    .AddParameter("count", 10)
            //    .Add();

            //// task to run every other Monday starting from Jan 01, 2016
            //config.ScheduledTask<TestAsyncTask>("Monday task", LogType.All)
            //    .RunWeekly()
            //    .OnWeekdays(Weekdays.Monday)
            //    .RepeatEvery(2) // every other day
            //    .StartsFrom(DateTime.Parse("01/01/2016")) //reference point to skip every other day
            //    .RunsAt(TimeSpan.Parse("06:00"))
            //    .OfLocalTimeZone()
            //    .AddParameter("count", 10)
            //    .Add();


            //config.TimerTask<TestAsyncTask>("Every 15 mins", LogType.Error)
            //    .RunEvery(900000)
            //    .RepeatFor(-1) //for continue, you can limit number of time the task should run
            //    .OnWeekdays(Weekdays.Monday | Weekdays.Friday) // Run on Mondays and Fridays
            //    .DuringPeriod(DateTime.Parse("01/01/2016"), DateTime.Parse("12/31/2016")) // life time of the task
            //    .DuringTimeOfDay(TimeSpan.Parse("00:00"), TimeSpan.Parse("06:00")) // valid time during the day. Here it from mid-night thru 6 AM
            //    .OfLocalTimeZone()
            //    .AddParameter("task", 50)
            //    .AddParameter("other", "other parameter")
            //    .Add();

            ////Run parallele task to process requests/transactions etc.
            //config.ParallelTask<TestAsyncTask>("Parallel instances", LogType.Information)
            //    .RunWithMinimumInstancesOf(1)
            //    .AndMaximumInstancesOf(50)
            //    .WithIdlePeriod(2000) // poll every 2 seconds when there is nothing to process
            //    .OnWeekdays(Weekdays.Monday | Weekdays.Tuesday | Weekdays.Wednesday | Weekdays.Thursday | Weekdays.Friday) // weekdays
            //    .DuringTimeOfDay(TimeSpan.Parse("08:00"), TimeSpan.Parse("17:00")) // during work hours
            //    .OfUtcTimeZone() // UTC timezone
            //    .AddParameter("task", 50)
            //    .AddParameter("other", "other parameter")
            //    .Add();


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
