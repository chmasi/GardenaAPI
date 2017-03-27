using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.NET.Models
{
    public class Login
    {
        [JsonProperty("sessions")]
        public Credentials sessions { get; set; }
    }

    public class Credentials
    {
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("password")]
        public string password { get; set; }
        [JsonProperty("token")]
        public string token { get; set; }
        [JsonProperty("user_id")]
        public string user_id { get; set; }
        [JsonProperty("status_message")]
        public string status_message { get; set; }
    }
}
