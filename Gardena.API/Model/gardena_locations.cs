using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.Net.Model
{
    public class Attributes
    {
        public string name { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string type { get; set; }
        public Attributes attributes { get; set; }
    }

    public class gardena_locations
    {
        public List<Datum> data { get; set; }
    }
}
