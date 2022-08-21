using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.Net.Model
{

    public partial class gardena_locations_id
    {
        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("included")]
        public List<Included> Included { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("relationships")]
        public DataRelationships Relationships { get; set; }

        [JsonProperty("attributes")]
        public DataAttributes Attributes { get; set; }
    }

    public partial class DataAttributes
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class DataRelationships
    {
        [JsonProperty("devices")]
        public Vices Devices { get; set; }
    }

    public partial class Vices
    {
        [JsonProperty("data")]
        public List<Dat> Data { get; set; }
    }

    public partial class Dat
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Included
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("relationships")]
        public IncludedRelationships Relationships { get; set; }

        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public IncludedAttributes Attributes { get; set; }
    }

    public partial class IncludedAttributes
    {
        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public Activity State { get; set; }

        [JsonProperty("activity", NullValueHandling = NullValueHandling.Ignore)]
        public Activity Activity { get; set; }

        [JsonProperty("lastErrorCode", NullValueHandling = NullValueHandling.Ignore)]
        public Activity LastErrorCode { get; set; }

        [JsonProperty("operatingHours", NullValueHandling = NullValueHandling.Ignore)]
        public OperatingHours OperatingHours { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public ModelType Name { get; set; }

        [JsonProperty("batteryLevel", NullValueHandling = NullValueHandling.Ignore)]
        public Level BatteryLevel { get; set; }

        [JsonProperty("batteryState", NullValueHandling = NullValueHandling.Ignore)]
        public Activity BatteryState { get; set; }

        [JsonProperty("rfLinkLevel", NullValueHandling = NullValueHandling.Ignore)]
        public Level RfLinkLevel { get; set; }

        [JsonProperty("serial", NullValueHandling = NullValueHandling.Ignore)]
        public ModelType Serial { get; set; }

        [JsonProperty("modelType", NullValueHandling = NullValueHandling.Ignore)]
        public ModelType ModelType { get; set; }

        [JsonProperty("rfLinkState", NullValueHandling = NullValueHandling.Ignore)]
        public ModelType RfLinkState { get; set; }
    }

    public partial class Activity
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }

    public partial class Level
    {
        [JsonProperty("value")]
        public long Value { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }

    public partial class ModelType
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class OperatingHours
    {
        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public partial class IncludedRelationships
    {
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public Device Location { get; set; }

        [JsonProperty("services", NullValueHandling = NullValueHandling.Ignore)]
        public Vices Services { get; set; }

        [JsonProperty("device", NullValueHandling = NullValueHandling.Ignore)]
        public Device Device { get; set; }
    }

    public partial class Device
    {
        [JsonProperty("data")]
        public Dat Data { get; set; }
    }
}
