using lb_QueueServices.Domain.Models;

namespace lb_QueueServices.Domain.Contracts
{
    /// <summary>
    /// Contrato para publicar mensajes en un exchange.
    /// </summary>
    public interface IQueuePublisher
    {
        /// <summary>
        /// Asegura que el exchange exista para el contexto proporcionado.
        /// </summary>
        Task EnsureExchangeAsync(QueueContext context);

        /// <summary>
        /// Publica un mensaje con la configuracion del contexto.
        /// </summary>
        Task PublishAsync<T>(T message, QueueContext context);
    }
}
