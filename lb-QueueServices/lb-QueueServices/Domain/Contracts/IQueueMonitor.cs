using lb_QueueServices.Domain.Events;
using lb_QueueServices.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace lb_QueueServices.Domain.Contracts
{
    /// <summary>
    /// Monitor pasivo (NO ACK / NO consumo)
    /// </summary>
    public interface IQueueMonitor
    {
        event EventHandler<QueueMessageReceivedEvent>? MessageObserved;

        Task StartMonitoringAsync(QueueContext context);
        Task StopMonitoringAsync();
    }
}
