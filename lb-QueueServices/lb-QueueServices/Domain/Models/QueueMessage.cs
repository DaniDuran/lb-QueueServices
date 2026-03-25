namespace lb_QueueServices.Domain.Models
{
    /// <summary>
    /// Represents a message delivered from a queue.
    /// </summary>
    public class QueueMessage
    {
        /// <summary>
        /// Message payload as text.
        /// </summary>
        public string Payload { get; init; } = string.Empty;

        /// <summary>
        /// Delivery tag assigned by the broker.
        /// </summary>
        public ulong DeliveryTag { get; init; }

        /// <summary>
        /// Routing key used for the delivery.
        /// </summary>
        public string RoutingKey { get; init; } = string.Empty;
    }
}
