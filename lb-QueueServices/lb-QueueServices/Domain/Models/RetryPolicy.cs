namespace lb_QueueServices.Domain.Models
{
    /// <summary>
    /// Configuracion de reintentos usada por los consumidores.
    /// </summary>
    public sealed class RetryPolicy
    {
        /// <summary>
        /// Maximo numero de reintentos antes de rechazar.
        /// </summary>
        public int MaxRetries { get; init; } = 5;

        /// <summary>
        /// Demora entre reintentos en milisegundos.
        /// </summary>
        public int DelayMilliseconds { get; init; } = 30000;

        /// <summary>
        /// Sufijo para exchanges de reintento.
        /// </summary>
        public string RetryExchangeSuffix { get; init; } = ".retry";

        /// <summary>
        /// Sufijo para exchanges dead-letter.
        /// </summary>
        public string DeadLetterExchangeSuffix { get; init; } = ".dlq";
    }
}
