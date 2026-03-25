using RabbitMQ.Client;
using System.Collections.Generic;

namespace lb_QueueServices.Domain.Models
{
    /// <summary>
    /// Configuration for connecting to RabbitMQ and defining queue topology.
    /// </summary>
    public sealed class QueueContext
    {
        /// <summary>
        /// Broker host name.
        /// </summary>
        public string Host { get; init; } = default!;

        /// <summary>
        /// Broker port (default 5672).
        /// </summary>
        public int Port { get; init; } = 5672;

        /// <summary>
        /// Username for the connection.
        /// </summary>
        public string User { get; init; } = default!;

        /// <summary>
        /// Password for the connection.
        /// </summary>
        public string Password { get; init; } = default!;

        /// <summary>
        /// Exchange name.
        /// </summary>
        public string Exchange { get; init; } = default!;

        /// <summary>
        /// Exchange type (e.g. ExchangeType.Topic or ExchangeType.Fanout).
        /// </summary>
        public string ExchangeType { get; set; } = default!;

        /// <summary>
        /// Optional queue name; leave empty for fanout publish only.
        /// </summary>
        public string? Queue { get; init; } = default!;

        /// <summary>
        /// Routing key for the binding and publish.
        /// </summary>
        public string RoutingKey { get; init; } = default!;

        /// <summary>
        /// Persistence flag used when building basic properties.
        /// </summary>
        public bool Persistent { get; init; } = true;

        /// <summary>
        /// Message priority used when building basic properties.
        /// </summary>
        public byte Priority { get; init; } = default!;

        /// <summary>
        /// Enable basic QoS settings for the consumer.
        /// </summary>
        public bool UseBasicQos { get; init; } = false;

        /// <summary>
        /// Prefetch size for basic QoS.
        /// </summary>
        public uint BasicQosPrefetchSize { get; init; } = default!;

        /// <summary>
        /// Prefetch count for basic QoS.
        /// </summary>
        public ushort BasicQosPrefetchCount { get; init; } = default!;

        /// <summary>
        /// Apply QoS settings globally on the channel.
        /// </summary>
        public bool BasicQosGlobal { get; init; } = false;

        /// <summary>
        /// Queue declaration arguments (e.g. dead-letter settings).
        /// </summary>
        public Dictionary<string, object?> Arguments { get; init; } = new();

        /// <summary>
        /// Optional basic properties to use during publish.
        /// </summary>
        public BasicProperties? BasicProperties { get; init; }
    }
}
