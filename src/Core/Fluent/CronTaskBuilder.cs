using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent
{
    public interface ICronTaskBuilder: ITaskBuilder
    {
        ICronTaskBuilder RunEvery(long milliSeconds);
        ICronTaskBuilder Repeat(long times);
        ICronTaskBuilder DuringPeriod(DateTime from, DateTime to);
        ICronTaskBuilder DuringTime(TimeSpan from, TimeSpan to);
        ICronTaskBuilder OnWeekDays(Weekdays weekdays);
        ICronTaskBuilder ForTimezone(TimeZoneInfo timezone);
        ICronTaskBuilder WithTrace(TraceType traceType);
        void Add();
    }
    internal class CronTaskBuilder: TaskBuilder, ICronTaskBuilder
    {
        private long _interval;
        private long _recurrence;
        private DateTime _startDateTime = DateTime.MinValue;
        private DateTime _endDateTime = DateTime.MaxValue;
        private TimeSpan _dayStartTime = TimeSpan.FromSeconds(0);
        private TimeSpan _dayEndTime = TimeSpan.FromSeconds(86399);
        private Weekdays _weekdays = Weekdays.All;
        private TimeZoneInfo _timeZoneInfo = TimeZoneInfo.Local;
        internal CronTaskBuilder(FlowConfiguration config, string name, Type taskType)
            : base(config, name, taskType) { }

        public override void Add()
        {
            Config.InternalRunners.Add(new TimerTaskRunner(Name, TaskType, Parameters, _interval, _recurrence, _weekdays, _startDateTime, _endDateTime, _dayStartTime, _dayEndTime, _timeZoneInfo, TraceType));
        }

        public ICronTaskBuilder RunEvery(long milliSeconds)
        {
            _interval = milliSeconds;
            return this;
        }

        public ICronTaskBuilder Repeat(long times)
        {
            _recurrence = times;
            return this;
        }

        public ICronTaskBuilder DuringPeriod(DateTime from, DateTime to)
        {
            _startDateTime = from;
            _endDateTime = to;
            return this;
        }

        public ICronTaskBuilder DuringTime(TimeSpan from, TimeSpan to)
        {
            _dayStartTime = from;
            _dayEndTime = to;
            return this;
        }

        public ICronTaskBuilder OnWeekDays(Weekdays weekdays)
        {
            _weekdays = weekdays;
            return this;
        }

        public ICronTaskBuilder ForTimezone(TimeZoneInfo timezone)
        {
            throw new NotImplementedException();
        }

        public ICronTaskBuilder WithTrace(TraceType traceType)
        {
            TraceType = traceType;
            return this;
        }
    }
}
