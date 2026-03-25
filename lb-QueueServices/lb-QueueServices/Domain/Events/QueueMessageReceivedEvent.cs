namespace lb_QueueServices.Domain.Events
{
    /// <summary>
    /// Evento que se dispara cuando un mensaje es entregado al consumidor.
    /// </summary>
    public class QueueMessageReceivedEvent
    {
        /// <summary>
        /// Nombre de la cola o routing key usado en la entrega.
        /// </summary>
        public string QueueName { get; }

        /// <summary>
        /// Bytes crudos del cuerpo del mensaje.
        /// </summary>
        public byte[] Body { get; }

        /// <summary>
        /// Headers del mensaje, si existen.
        /// </summary>
        public IReadOnlyDictionary<string, object>? Headers { get; }

        private readonly Func<ValueTask> _ack;
        private readonly Func<bool, ValueTask> _nack;

        /// <summary>
        /// Crea un evento con callbacks de ack/nack.
        /// </summary>
        /// <param name="queueName">Nombre de cola o routing key.</param>
        /// <param name="body">Bytes del mensaje.</param>
        /// <param name="ack">Callback para confirmar el mensaje.</param>
        /// <param name="nack">Callback para rechazar el mensaje (opcionalmente reencolar).</param>
        /// <param name="headers">Headers opcionales.</param>
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
        /// Confirma el mensaje como procesado correctamente.
        /// </summary>
        public ValueTask AckAsync() => _ack();

        /// <summary>
        /// Rechaza el mensaje y opcionalmente lo reencola.
        /// </summary>
        /// <param name="requeue">Si debe reencolarse.</param>
        public ValueTask NackAsync(bool requeue) => _nack(requeue);

        /// <summary>
        /// Lee el conteo de reintentos desde el header "x-retry-count".
        /// Retorna 0 si no existe o es invalido.
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
