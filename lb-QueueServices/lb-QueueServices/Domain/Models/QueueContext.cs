using System;
using System.Collections.Generic;
using System.Text;

namespace lb_QueueServices.Domain.Models
{
    public sealed class QueueContext
    {
        public string Host { get; init; } = default!;
        public int Port { get; init; } = 5672;
        public string User { get; init; } = default!;
        public string Password { get; init; } = default!;

        public string Exchange { get; init; } = default!;
        public string ExchangeType { get; set; } = default!;

        public string? Queue { get; init; } = default!;
        public string RoutingKey { get; init; } = default!;
    }
}
