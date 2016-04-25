using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common;
using XecMe.Core.Tasks;
using XecMe.Core.Utils;

namespace XecMe.Core.Fluent
{
    public interface IParallelTaskBuilder: ITaskBuilder
    {
        IParallelTaskBuilder WithLeastThreads(uint min);
        IParallelTaskBuilder WithMostThreads(uint max);
        IParallelTaskBuilder HasIdlePeriod(ulong period);
        IParallelTaskBuilder DuringTime(TimeSpan from, TimeSpan to);
        IParallelTaskBuilder OnWeekDays(Weekdays weekdays);
        IParallelTaskBuilder ForTimezone(TimeZoneInfo timezone);
        IParallelTaskBuilder WithTrace(TraceType traceType);
        void Add();

    }
    internal class ParallelTaskBuilder: TaskBuilder, IParallelTaskBuilder
    {
        uint _min, _max;
        ulong _period;
        TimeSpan _from, _to;
        Weekdays _weekdays;
        TimeZoneInfo _timezone;
        internal ParallelTaskBuilder(FlowConfiguration config, string name, Type taskType)
            :base(config, name, taskType)
        {
            _min = 1;
            _weekdays = Weekdays.All;
            _timezone = TimeZoneInfo.Local;
            _from = Time.DayMinTime;
            _to = Time.DayMaxTime;
        }

        public IParallelTaskBuilder WithLeastThreads(uint min)
        {
            _min = min;
            return this;
        }

        public IParallelTaskBuilder WithMostThreads(uint max)
        {
            _max = max;
            return this;
        }

        public IParallelTaskBuilder HasIdlePeriod(ulong period)
        {
            _period = period;
            return this;
        }

        public IParallelTaskBuilder DuringTime(TimeSpan from, TimeSpan to)
        {
            _from = from;
            _to = to;
            return this;
        }

        public IParallelTaskBuilder OnWeekDays(Weekdays weekdays)
        {
            _weekdays = weekdays;
            return this;
        }

        public IParallelTaskBuilder ForTimezone(TimeZoneInfo timezone)
        {
            timezone.NotNull(nameof(timezone));

            return this;
        }

        public IParallelTaskBuilder WithTrace(TraceType traceType)
        {
            TraceType = traceType;
            return this;
        }

        public override void Add()
        {
            Config.InternalRunners.Add(new ParallelTaskRunner(Name, TaskType, Parameters, _min, _max, _period, _from, _to, _weekdays, _timezone, TraceType));
        }
    }
}
