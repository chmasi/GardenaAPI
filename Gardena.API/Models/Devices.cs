using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.NET.Models
{
    public class Property
    {
        public string id { get; set; }
        public string name { get; set; }
        public object value { get; set; }
        public bool writeable { get; set; }
        public List<object> supported_values { get; set; }
        public string timestamp { get; set; }
        public string unit { get; set; }
    }

    public class Ability
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<Property> properties { get; set; }
        public string type { get; set; }
    }

    public class ConfigurationSynchronizedV2
    {
        public bool value { get; set; }
        public string timestamp { get; set; }
    }

    public class Device
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public bool configuration_synchronized { get; set; }
        public List<Ability> abilities { get; set; }
        public ConfigurationSynchronizedV2 configuration_synchronized_v2 { get; set; }
        public List<object> constraints { get; set; }
        public List<object> scheduled_events { get; set; }
        public List<object> settings { get; set; }
        public List<object> status_report_history { get; set; }
        public List<object> zones { get; set; }
    }

    public class Devices
    {
        public List<Device> devices { get; set; }
        [JsonProperty("status_message")]
        public string status_message { get; set; }
    }
}
