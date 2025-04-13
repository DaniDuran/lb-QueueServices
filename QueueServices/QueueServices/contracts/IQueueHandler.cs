using QueueServices.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace QueueServices.contracts
{
    public interface IQueueHandler: IDisposable
    {
        event EventHandler<ReceivedEventArgs>? ReceivedEvent;
        event EventHandler<ReceivedSetupFailedEventArgs>? ReceivedSetupFailedEvent;
        event EventHandler<ReceivedStartedEventArgs>? ReceivedStartedEvent;
        event EventHandler<EmitterEventArgs>? EmitterEvent;

        Task StopConsume();
        Task StartConsume(MQSettings mQSettings, string queueName);
        Task SendMessageAsync<T>(T data, MQSettings mQSettings, string queueName);
        Task SendMessageAsync(string payload, MQSettings mQSettings, string queueName);


    }
}
