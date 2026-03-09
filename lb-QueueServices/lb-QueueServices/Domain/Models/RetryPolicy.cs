using System;
using System.Collections.Generic;
using System.Text;

namespace lb_QueueServices.Domain.Models
{
    public sealed class RetryPolicy
    {
        public int MaxRetries { get; init; } = 5;
        public int DelayMilliseconds { get; init; } = 30000;
        public string RetryExchangeSuffix { get; init; } = ".retry";
        public string DeadLetterExchangeSuffix { get; init; } = ".dlq";
    }
}
