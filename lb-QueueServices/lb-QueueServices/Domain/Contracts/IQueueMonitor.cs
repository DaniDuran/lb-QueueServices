using lb_QueueServices.Domain.Events;
using lb_QueueServices.Domain.Models;
using System;

namespace lb_QueueServices.Domain.Contracts
{
    /// <summary>
    /// Passive monitor (no ack and no consumption).
    /// </summary>
    public interface IQueueMonitor
    {
        /// <summary>
        /// Fired when a message is observed.
        /// </summary>
        event EventHandler<QueueMessageReceivedEvent>? MessageObserved;

        /// <summary>
        /// Starts passive monitoring with the provided context.
        /// </summary>
        Task StartMonitoringAsync(QueueContext context);

        /// <summary>
        /// Stops passive monitoring and releases resources.
        /// </summary>
        Task StopMonitoringAsync();
    }
}
