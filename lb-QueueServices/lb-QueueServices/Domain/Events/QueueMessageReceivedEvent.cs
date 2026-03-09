using System;
using System.Collections.Generic;
using System.Text;

namespace lb_QueueServices.Domain.Events
{
    public class QueueMessageReceivedEvent
    {
        public string QueueName { get; }
        public byte[] Body { get; }
        public IDictionary<string, object>? Headers { get; }

        public QueueMessageReceivedEvent(
            string queueName,
            byte[] body,
            IDictionary<string, object>? headers = null)
        {
            QueueName = queueName;
            Body = body;
            Headers = headers;
        }
    }
}

