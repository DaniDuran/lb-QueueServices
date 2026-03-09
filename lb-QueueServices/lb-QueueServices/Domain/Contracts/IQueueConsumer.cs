using lb_QueueServices.Domain.Events;
using lb_QueueServices.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace lb_QueueServices.Domain.Contracts
{
    public interface IQueueConsumer : IDisposable
    {
        //event EventHandler<QueueMessageReceivedEvent>? MessageReceived;
        event Func<object?, QueueMessageReceivedEvent, Task> MessageReceived;
        event EventHandler<QueueErrorEvent>? Error;

        Task StartAsync(QueueContext context, RetryPolicy? retry = null);
        Task StopAsync();
    }
}
