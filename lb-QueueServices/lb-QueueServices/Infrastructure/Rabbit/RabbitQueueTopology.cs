using lb_QueueServices.Domain.Models;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace lb_QueueServices.Infrastructure.Rabbit
{
    /// <summary>
    /// Metodos de ayuda para declarar exchanges, colas y bindings.
    /// </summary>
    public static class RabbitQueueTopology
    {
        /// <summary>
        /// Declara exchange, cola y binding. Si se proporciona politica de reintento,
        /// configura un dead-letter exchange para reintentos.
        /// </summary>
        /// <param name="channel">Canal abierto para declarar.</param>
        /// <param name="ctx">Contexto de cola.</param>
        /// <param name="retry">Politica de reintentos opcional.</param>
        public static async Task DeclareAsync(
            IChannel channel,
            QueueContext ctx,
            RetryPolicy? retry = null)
        {
            await channel.ExchangeDeclareAsync(
                ctx.Exchange,
                type: ctx.ExchangeType,
                durable: true);

            IDictionary<string, object?>? args = retry is null
                ? null
                : new Dictionary<string, object?>
                {
                    { "x-dead-letter-exchange", $"{ctx.Exchange}{retry.RetryExchangeSuffix}" }
                };

            await channel.QueueDeclareAsync(
                queue: ctx.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: args);

            await channel.QueueBindAsync(
                queue: ctx.Queue,
                exchange: ctx.Exchange,
                routingKey: ctx.RoutingKey);
        }
    }
}
