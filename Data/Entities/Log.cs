using System;
using System.Collections.Generic;
using System.Text;

namespace UMC.Data.Entities
{
    public class Log
    {
        public string LogKey { get; set; }
        public DateTime? Time { get; set; }
        public string Caption { get; set; }

        public string Username { get; set; }
        public string IPAddress { get; set; }

        public string UserAgent { get; set; }
        public string Context { get; set; }
    }
}
