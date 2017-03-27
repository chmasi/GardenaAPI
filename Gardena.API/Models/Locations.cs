using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.NET.Models
{

    public class Locations
    {
        [JsonProperty("locations")]
        public IList<LocationProperty> locations { get; set; }
        [JsonProperty("status_message")]
        public string status_message { get; set; }
    }

    public class LocationProperty
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("devices")]
        public IList<string> devices { get; set; }

        [JsonProperty("zones")]
        public IList<string> zones { get; set; }
    }
}
