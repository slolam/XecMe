using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace XecMeConfig.Entities
{
    public class TimeZone
    {
        private static ReadOnlyCollection<TimeZone> _list;
        static TimeZone()
        {
            List<TimeZone> list = new List<TimeZone>();
            list.Add(new TimeZone { Id= null, Name="Default (machine's time zone)" });

            foreach (TimeZoneInfo item in TimeZoneInfo.GetSystemTimeZones())
            {
                list.Add(new TimeZone { Id = item.Id, Name = item.DisplayName });
            }

            _list = list.AsReadOnly();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public static ReadOnlyCollection<TimeZone> TimeZones
        {
            get
            {
                return _list;
            }
        }
    }
}
