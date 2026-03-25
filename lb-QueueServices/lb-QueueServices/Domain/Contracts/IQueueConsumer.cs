using lb_QueueServices.Domain.Events;
using lb_QueueServices.Domain.Models;
using System;

namespace lb_QueueServices.Domain.Contracts
{
    /// <summary>
    /// Contrato para consumir mensajes de una cola con control explicito de ack/nack.
    /// </summary>
    public interface IQueueConsumer : IDisposable
    {
        //event EventHandler<QueueMessageReceivedEvent>? MessageReceived;
        /// <summary>
        /// Se dispara cuando se entrega un mensaje al consumidor.
        /// Llame a <see cref="QueueMessageReceivedEvent.AckAsync"/> o
        /// <see cref="QueueMessageReceivedEvent.NackAsync"/> para completar el procesamiento.
        /// </summary>
        event Func<object?, QueueMessageReceivedEvent, Task> MessageReceived;

        /// <summary>
        /// Se dispara cuando el consumidor encuentra un error.
        /// </summary>
        event EventHandler<QueueErrorEvent>? Error;

        /// <summary>
        /// Inicia el consumo con el contexto y una politica de reintentos opcional.
        /// </summary>
        Task StartAsync(QueueContext context, RetryPolicy? retry = null);

        /// <summary>
        /// Detiene el consumo y libera recursos.
        /// </summary>
        Task StopAsync();
    }
}
