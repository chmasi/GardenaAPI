using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.NET.Models
{
    public class Recurrence
    {
        public string type { get; set; }
        public List<string> weekdays { get; set; }
    }

    public class ScheduledEvent
    {
        public string id { get; set; }
        public string start_at { get; set; }
        public string end_at { get; set; }
        public string type { get; set; }
        public Recurrence recurrence { get; set; }
        public string weekday { get; set; }
    }

    public class Schedule
    {
        public ScheduledEvent scheduled_event { get; set; }
    }
}
