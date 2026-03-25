namespace lb_QueueServices.Domain.Events
{
    /// <summary>
    /// Event raised when a queue message is delivered to a consumer.
    /// </summary>
    public class QueueMessageReceivedEvent
    {
        /// <summary>
        /// Queue name or routing key used for delivery.
        /// </summary>
        public string QueueName { get; }

        /// <summary>
        /// Raw message body bytes.
        /// </summary>
        public byte[] Body { get; }

        /// <summary>
        /// Message headers, if any.
        /// </summary>
        public IReadOnlyDictionary<string, object>? Headers { get; }

        private readonly Func<ValueTask> _ack;
        private readonly Func<bool, ValueTask> _nack;

        /// <summary>
        /// Creates a new message event with ack/nack callbacks.
        /// </summary>
        /// <param name="queueName">Queue name or routing key.</param>
        /// <param name="body">Message body bytes.</param>
        /// <param name="ack">Callback to acknowledge the message.</param>
        /// <param name="nack">Callback to reject the message (optionally requeue).</param>
        /// <param name="headers">Optional delivery headers.</param>
        public QueueMessageReceivedEvent(
            string queueName,
            byte[] body,
            Func<ValueTask> ack,
            Func<bool, ValueTask> nack,
            IReadOnlyDictionary<string, object>? headers = null)
        {
            QueueName = queueName;
            Body = body;
            Headers = headers;
            _ack = ack;
            _nack = nack;
        }

        /// <summary>
        /// Acknowledges the message as successfully processed.
        /// </summary>
        public ValueTask AckAsync() => _ack();

        /// <summary>
        /// Rejects the message and optionally requeues it.
        /// </summary>
        /// <param name="requeue">Whether to requeue the message.</param>
        public ValueTask NackAsync(bool requeue) => _nack(requeue);

        /// <summary>
        /// Reads the retry count from the "x-retry-count" header.
        /// Returns 0 when not present or invalid.
        /// </summary>
        public int GetRetryCount()
        {
            if (Headers == null) return 0;

            if (Headers.TryGetValue("x-retry-count", out var value))
            {
                if (value is byte[] bytes)
                {
                    var str = System.Text.Encoding.UTF8.GetString(bytes);
                    if (int.TryParse(str, out var retry))
                        return retry;
                }

                if (value is int retryInt)
                    return retryInt;
            }

            return 0;
        }
    }
}
