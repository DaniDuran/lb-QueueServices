using lb_QueueServices.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace lb_QueueServices.Domain.Contracts
{
    public interface IQueuePublisher
    {
        Task EnsureExchangeAsync(QueueContext context);
        Task PublishAsync<T>(T message, QueueContext context);
    }
}
