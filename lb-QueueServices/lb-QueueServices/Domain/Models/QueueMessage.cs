namespace lb_QueueServices.Domain.Models
{
    /// <summary>
    /// Representa un mensaje entregado desde una cola.
    /// </summary>
    public class QueueMessage
    {
        /// <summary>
        /// Contenido del mensaje como texto.
        /// </summary>
        public string Payload { get; init; } = string.Empty;

        /// <summary>
        /// Delivery tag asignado por el broker.
        /// </summary>
        public ulong DeliveryTag { get; init; }

        /// <summary>
        /// Routing key usado en la entrega.
        /// </summary>
        public string RoutingKey { get; init; } = string.Empty;
    }
}
