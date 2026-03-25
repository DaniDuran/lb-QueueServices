namespace lb_QueueServices.Domain.Models
{
    /// <summary>
    /// Retry policy configuration used by consumers.
    /// </summary>
    public sealed class RetryPolicy
    {
        /// <summary>
        /// Maximum number of retries before rejecting the message.
        /// </summary>
        public int MaxRetries { get; init; } = 5;

        /// <summary>
        /// Delay between retries in milliseconds.
        /// </summary>
        public int DelayMilliseconds { get; init; } = 30000;

        /// <summary>
        /// Suffix for retry exchange names.
        /// </summary>
        public string RetryExchangeSuffix { get; init; } = ".retry";

        /// <summary>
        /// Suffix for dead-letter exchange names.
        /// </summary>
        public string DeadLetterExchangeSuffix { get; init; } = ".dlq";
    }
}
