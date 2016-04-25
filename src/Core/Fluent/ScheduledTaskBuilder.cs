using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent
{
    public interface IScheduledTaskBuilder
    {
        IScheduledTaskBuilder DuringPeriod(DateTime start, DateTime end);
        IScheduledTaskBuilder TaskTime(TimeSpan taskTime);

        IDailyScheduledTaskBuilder Daily();

        IWeeklyScheduledTaskBuilder Weekly();

        IMonthlyByDayScheduledTaskBuilder MonthlyByDays();

        IMonthlyByWeekdaysScheduledTaskBuilder MonthlyByWeeksAndWeekDays();
    }

    public interface IDailyScheduledTaskBuilder: ITaskBuilder
    {
        IScheduledTaskBuilder Repeat(int days);
    }


    public interface IWeeklyScheduledTaskBuilder: ITaskBuilder
    {
        IWeeklyScheduledTaskBuilder OnWeekdays(Weekdays weekdays);
        IWeeklyScheduledTaskBuilder Repeat(int weeks);
    }

    public interface IMonthlyByDayScheduledTaskBuilder: ITaskBuilder
    {
        IMonthlyByDayScheduledTaskBuilder Months(Months months);

        IMonthlyByDayScheduledTaskBuilder Days(params int[] days);
    }

    public interface IMonthlyByWeekdaysScheduledTaskBuilder: ITaskBuilder
    {
        IMonthlyByWeekdaysScheduledTaskBuilder Months(Months months);

        IMonthlyByWeekdaysScheduledTaskBuilder Weekdays(Weekdays weekdays);

        IMonthlyByWeekdaysScheduledTaskBuilder Weeks(Week weeks);
    }

    internal class ScheduledTaskBuilder: TaskBuilder, IScheduledTaskBuilder, IDailyScheduledTaskBuilder, IWeeklyScheduledTaskBuilder, IMonthlyByDayScheduledTaskBuilder, IMonthlyByWeekdaysScheduledTaskBuilder
    {
        Recursion _recursion;
        private DateTime _startDate, _lastDateTime;
        private TimeSpan _taskTime;
        private int _repeat;
        uint _days = 0;
        Week _weeks = Week.None;
        Months _months = Tasks.Months.None;
        Weekdays _weekdays = Tasks.Weekdays.None;
        internal ScheduledTaskBuilder(FlowConfiguration config, string name, Type taskType)
            : base(config, name, taskType) { }

        public override void Add()
        {
            IRecur recur = null;
            switch (_recursion)
            {
                case Recursion.Daily:
                    recur = new Daily(_repeat);
                    break;
                case Recursion.Weekly:
                    recur = new Weekly(_repeat, _weekdays);
                    break;
                case Recursion.Monthly:
                    if (_weeks != Week.None && _weekdays != Tasks.Weekdays.None && _months != Tasks.Months.None)
                        recur = new MonthlyByWeekdays(_months, _weekdays, _weeks);
                    else if (_days > 1 && _months != Tasks.Months.None)
                        recur = new MonthlyByDay(_months, _days);
                    else
                        throw new InvalidOperationException($"Missing the attributes for the Monthly task");
                    break;
                default:
                    break;
            }


        }

        public IScheduledTaskBuilder DuringPeriod(DateTime start, DateTime end)
        {
            _startDate = start;
            _lastDateTime = end;
            return this;
        }

        public IScheduledTaskBuilder TaskTime(TimeSpan taskTime)
        {
            _taskTime = taskTime;
            return this;
        }

        public IDailyScheduledTaskBuilder Daily()
        {
            return this;
        }

        public IWeeklyScheduledTaskBuilder Weekly()
        {
            return this;
        }

        public IMonthlyByDayScheduledTaskBuilder MonthlyByDays()
        {
            return this;
        }

        public IMonthlyByWeekdaysScheduledTaskBuilder MonthlyByWeeksAndWeekDays()
        {
            return this;
        }

        public IScheduledTaskBuilder Repeat(int days)
        {
            _repeat = days;
            return this;
        }

        public IWeeklyScheduledTaskBuilder OnWeekdays(Weekdays weekdays)
        {
            _weekdays = weekdays;
            return this;
        }

        IWeeklyScheduledTaskBuilder IWeeklyScheduledTaskBuilder.Repeat(int weeks)
        {
            _repeat = weeks;
            return this;
        }

        

        public IMonthlyByDayScheduledTaskBuilder Months(Months months)
        {
            return this;
        }

        public IMonthlyByDayScheduledTaskBuilder Days(params int[] days)
        {
            return this;
        }

        IMonthlyByWeekdaysScheduledTaskBuilder IMonthlyByWeekdaysScheduledTaskBuilder.Months(Months months)
        {
            return this;
        }

        public IMonthlyByWeekdaysScheduledTaskBuilder Weekdays(Weekdays weekdays)
        {
            return this;
        }

        public IMonthlyByWeekdaysScheduledTaskBuilder Weeks(Week weeks)
        {
            return this;
        }
    }
}
