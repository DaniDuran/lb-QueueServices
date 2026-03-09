using System;
using System.Collections.Generic;
using System.Text;

namespace lb_QueueServices.Domain.Models
{
    public class QueueMessage
    {
        public string Payload { get; init; } = string.Empty;
        public ulong DeliveryTag { get; init; }
        public string RoutingKey { get; init; } = string.Empty;
    }

}
