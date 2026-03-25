using lb_QueueServices.Domain.Models;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace lb_QueueServices.Infrastructure.Rabbit
{
    /// <summary>
    /// Helper methods for declaring exchanges, queues, and bindings.
    /// </summary>
    public static class RabbitQueueTopology
    {
        /// <summary>
        /// Declares exchange, queue, and binding. When a retry policy is provided,
        /// it configures a dead-letter exchange for retries.
        /// </summary>
        /// <param name="channel">Open channel to declare against.</param>
        /// <param name="ctx">Queue context.</param>
        /// <param name="retry">Optional retry policy.</param>
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
