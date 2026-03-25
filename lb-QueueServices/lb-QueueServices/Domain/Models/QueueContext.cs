using RabbitMQ.Client;
using System.Collections.Generic;

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

        public bool Persistent { get; init; } = true;
        public byte Priority { get; init; } = default!;

        public bool UseBasicQos { get; init; } = false;
        public uint BasicQosPrefetchSize { get; init; } = default!;
        public ushort BasicQosPrefetchCount { get; init; } = default!;
        public bool BasicQosGlobal { get; init; } = false;

        public Dictionary<string, object?> Arguments { get; init; } = new();
        public BasicProperties? BasicProperties { get; init; }
    }
}
