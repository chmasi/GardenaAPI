using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.Net.Model
{
    class gadena_put
    {
    }
    public partial class Command
    {
        [JsonProperty("putdata")]
        public putData putData { get; set; }
    }

    public partial class putData
    {
        [JsonProperty("putid")]
        public string putId { get; set; }

        [JsonProperty("puttype")]
        public string putType { get; set; }

        [JsonProperty("putattributes")]
        public putAttributes putAttributes { get; set; }
    }

    public partial class putAttributes
    {
        [JsonProperty("putcommand")]
        public string putCommand { get; set; }

        [JsonProperty("putseconds")]
        public long putSeconds { get; set; }
    }
}
