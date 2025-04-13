using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServices.dtos
{
    public class EmitterEventArgs
    {
        public bool Success { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}
