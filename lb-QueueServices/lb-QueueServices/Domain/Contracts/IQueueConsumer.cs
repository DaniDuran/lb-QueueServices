using lb_QueueServices.Domain.Events;
using lb_QueueServices.Domain.Models;
using System;

namespace lb_QueueServices.Domain.Contracts
{
    /// <summary>
    /// Contract for consuming messages from a queue with explicit ack/nack control.
    /// </summary>
    public interface IQueueConsumer : IDisposable
    {
        //event EventHandler<QueueMessageReceivedEvent>? MessageReceived;
        /// <summary>
        /// Fired when a message is delivered to the consumer.
        /// Call <see cref="QueueMessageReceivedEvent.AckAsync"/> or
        /// <see cref="QueueMessageReceivedEvent.NackAsync"/> to complete processing.
        /// </summary>
        event Func<object?, QueueMessageReceivedEvent, Task> MessageReceived;

        /// <summary>
        /// Fired when the consumer encounters an error.
        /// </summary>
        event EventHandler<QueueErrorEvent>? Error;

        /// <summary>
        /// Starts consuming messages with the provided context and optional retry policy.
        /// </summary>
        Task StartAsync(QueueContext context, RetryPolicy? retry = null);

        /// <summary>
        /// Stops consuming and releases resources.
        /// </summary>
        Task StopAsync();
    }
}
