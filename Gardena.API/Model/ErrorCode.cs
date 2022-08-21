using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.Net.Model
{
    public class Error
    {
        public string id { get; set; }
        public string status { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public string detail { get; set; }
    }

    public class ErrorCode
    {
        public List<Error> errors { get; set; }
    }

}
