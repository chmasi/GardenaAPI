using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.NET.Models
{
    public class CreateRecurrence
    {
        public string type { get; set; }
        public List<string> weekdays { get; set; }
    }

    public class CreateScheduledEvents
    {
        public string type { get; set; }
        public string start_at { get; set; }
        public string end_at { get; set; }
        public CreateRecurrence recurrence { get; set; }
        public string device { get; set; }
    }

    public class CreateSchedule
    {
        public CreateScheduledEvents scheduled_events { get; set; }
    }
}
