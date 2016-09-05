using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent
{
    class FluentInterfaces : IForMonths<IForWeeks<ITask>>, IForWeeks<ITask>
    {
        public IForWeeks<ITask> ForMonths(Months months)
        {
            throw new NotImplementedException();
        }

        public ITask ForWeeks(Weeks weeks)
        {
            throw new NotImplementedException();
        }
    }

    public interface IForMonths<out TI> 
    {
        TI ForMonths(Months months);
    }

    public interface IForWeeks<out TI> 
    {
        TI ForWeeks(Weeks weeks);
    }

    public interface IOnDays<out TI> 
    {
        TI OnDays(params uint[] days);
    }

    public interface IOnWeekdays<out TI> 
    {
        TI OnWeekdays(Weekdays weekdays);
    }

    public interface IRepeat<out TI> 
    {
        TI Repeat(uint repeat);
    }


}
