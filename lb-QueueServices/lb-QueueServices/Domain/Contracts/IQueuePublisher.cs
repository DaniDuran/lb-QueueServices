using lb_QueueServices.Domain.Models;

namespace lb_QueueServices.Domain.Contracts
{
    /// <summary>
    /// Contract for publishing messages to a queue exchange.
    /// </summary>
    public interface IQueuePublisher
    {
        /// <summary>
        /// Ensures the exchange exists for the provided context.
        /// </summary>
        Task EnsureExchangeAsync(QueueContext context);

        /// <summary>
        /// Publishes a message with the provided context configuration.
        /// </summary>
        Task PublishAsync<T>(T message, QueueContext context);
    }
}
