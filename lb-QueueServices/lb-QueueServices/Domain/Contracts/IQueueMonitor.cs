using lb_QueueServices.Domain.Events;
using lb_QueueServices.Domain.Models;
using System;

namespace lb_QueueServices.Domain.Contracts
{
    /// <summary>
    /// Monitor pasivo (sin ack y sin consumo).
    /// </summary>
    public interface IQueueMonitor
    {
        /// <summary>
        /// Se dispara cuando se observa un mensaje.
        /// </summary>
        event EventHandler<QueueMessageReceivedEvent>? MessageObserved;

        /// <summary>
        /// Inicia el monitoreo pasivo con el contexto proporcionado.
        /// </summary>
        Task StartMonitoringAsync(QueueContext context);

        /// <summary>
        /// Detiene el monitoreo pasivo y libera recursos.
        /// </summary>
        Task StopMonitoringAsync();
    }
}
