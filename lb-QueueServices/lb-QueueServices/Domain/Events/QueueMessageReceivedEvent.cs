namespace lb_QueueServices.Domain.Events
{
    public class QueueMessageReceivedEvent
    {
        public string QueueName { get; }
        public byte[] Body { get; }
        public IReadOnlyDictionary<string, object>? Headers { get; }
        private readonly Func<ValueTask> _ack;
        private readonly Func<bool, ValueTask> _nack;

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

        public ValueTask AckAsync() => _ack();
        public ValueTask NackAsync(bool requeue) => _nack(requeue);

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

