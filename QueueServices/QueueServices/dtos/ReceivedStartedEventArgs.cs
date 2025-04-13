using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServices.dtos
{
    public class ReceivedStartedEventArgs
    {
        public string QueueName { get; set; } = string.Empty;
    }
}
