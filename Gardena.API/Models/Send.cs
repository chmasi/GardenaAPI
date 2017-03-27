using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gardena.NET.Models
{
    public class Parameters
    {
        public int duration { get; set; }
    }

    public class SendCommand
    {
        public string name { get; set; }
        public Parameters parameters { get; set; }

    }
}