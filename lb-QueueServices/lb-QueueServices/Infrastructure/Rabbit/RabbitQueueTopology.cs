using lb_QueueServices.Domain.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace lb_QueueServices.Infrastructure.Rabbit
{
    public static class RabbitQueueTopology
    {        
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
