using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServices.dtos
{
    public class ReceivedSetupFailedEventArgs
    {
        public string QueueName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
