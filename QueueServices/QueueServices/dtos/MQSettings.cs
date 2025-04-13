using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServices.dtos
{
    public class MQSettings
    {
        public string Uid { get; set; } = string.Empty;
        public string Pwd { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
