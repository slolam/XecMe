#XecMe
**XecMe** in short for Execute Me, is a executions and hosting application block for the batches, background processes and Windows Services. It is a highly configurable and extensible framework. It follows the principles of task oriented design approach for solving the business problem.

## Fluent APIs
XecMe 2.0.0 has introduced fluent APIs for configuring the task. These are fairly straight forward to configure each type of task execution

```csharp

 var config = new FlowConfiguration();

//task to run on ThankGiving
 config.ScheduledTask<TestTask>("ThanksGiving task", LogType.All)
     .RunByWeeksOfTheMonths(Months.November)
     .OnWeeks(Weeks.Last)
     .OnWeekdays(Weekdays.Thursday)
     .RunsAt(TimeSpan.Parse("02:00"))
     .OfLocalTimeZone()
     .AddParameter("count", 10)
     .Add();

 //task to run on Christmas
 config.ScheduledTask<TestTask>("Christmas Email blaster", LogType.All)
     .RunByDaysOfTheMonths(Months.December)
     .OnDays(25)
     .RunsAt(TimeSpan.Parse("02:00"))
     .OfLocalTimeZone()
     .AddParameter("count", 10)
     .Add();

 //task to run every other day starting from Jan 01, 2016
 config.ScheduledTask<TestTask>("Alternate day task", LogType.All)
     .RunDaily()
     .RepeatEvery(2) // every other day
     .StartsFrom(DateTime.Parse("01/01/2016")) //reference point to skip every other day
     .RunsAt(TimeSpan.Parse("06:00"))
     .OfLocalTimeZone()
     .AddParameter("count", 10)
     .Add();

 // task to run every other Monday starting from Jan 01, 2016
 config.ScheduledTask<TestTask>("Monday task", LogType.All)
     .RunWeekly()
     .OnWeekdays(Weekdays.Monday)
     .RepeatEvery(2) // every other day
     .StartsFrom(DateTime.Parse("01/01/2016")) //reference point to skip every other day
     .RunsAt(TimeSpan.Parse("06:00"))
     .OfLocalTimeZone()
     .AddParameter("count", 10)
     .Add();
     
 config.TimerTask<TestTask>("Every 15 mins", LogType.Error)
     .RunEvery(900000)
     .RepeatFor(-1) //for continue, you can limit number of time the task should run
     .OnWeekdays(Weekdays.Monday | Weekdays.Friday) // Run on Mondays and Fridays
     .DuringPeriod(DateTime.Parse("01/01/2016"), DateTime.Parse("12/31/2016")) // life time of the task
     .DuringTimeOfDay(TimeSpan.Parse("00:00"), TimeSpan.Parse("06:00")) // valid time during the day. Here it from mid-night thru 6 AM
     .OfLocalTimeZone()
     .AddParameter("task", 50)
     .AddParameter("other", "other parameter")
     .Add();

//Run parallele task to process requests/transactions etc.
 config.ParallelTask<TestTask>("Parallel instances", LogType.Information)
     .RunWithMinimumInstancesOf(1)
     .AndMaximumInstancesOf(50)
     .WithIdlePeriod(2000) // poll every 2 seconds when there is nothing to process
     .OnWeekdays(Weekdays.Workdays) // weekdays
     .DuringTimeOfDay(TimeSpan.Parse("08:00"), TimeSpan.Parse("17:00")) // during work hours
     .OfUtcTimeZone() // UTC timezone
     .AddParameter("task", 50)
     .AddParameter("other", "other parameter")
     .Add();
```
