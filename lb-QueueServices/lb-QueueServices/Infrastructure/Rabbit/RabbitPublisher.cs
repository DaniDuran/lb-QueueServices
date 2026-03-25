using lb_QueueServices.Domain.Contracts;
using lb_QueueServices.Domain.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace lb_QueueServices.Infrastructure.Rabbit
{
    /// <summary>
    /// RabbitMQ publisher for declaring exchanges and publishing messages.
    /// </summary>
    public sealed class RabbitPublisher : IQueuePublisher
    {
        /// <summary>
        /// Ensures the exchange exists for the provided context.
        /// </summary>
        public async Task EnsureExchangeAsync(QueueContext context)
        {
            var factory = new RabbitConnectionFactory();

            using var connection = await factory.CreateConnectionAsync(context);
            using var channel = await connection.CreateChannelAsync();

            var exchangeType =
                string.IsNullOrWhiteSpace(context.RoutingKey)
                    ? ExchangeType.Fanout
                    : ExchangeType.Topic;

            await channel.ExchangeDeclareAsync(
                exchange: context.Exchange,
                type: exchangeType,
                durable: true,
                autoDelete: false);
        }

        private async Task EnsureQueueAndBindingAsync(QueueContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Queue))
                return; // fanout without explicit queue

            var factory = new RabbitConnectionFactory();

            using var connection = await factory.CreateConnectionAsync(context);
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: context.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await channel.QueueBindAsync(
                queue: context.Queue,
                exchange: context.Exchange,
                routingKey: context.RoutingKey ?? string.Empty);
        }

        /// <summary>
        /// Publishes a message and ensures the exchange and queue (if provided) exist.
        /// </summary>
        public async Task PublishAsync<T>(T message, QueueContext context)
        {
            var factory = new RabbitConnectionFactory();

            using var connection = await factory.CreateConnectionAsync(context);
            using var channel = await connection.CreateChannelAsync();

            // Exchange
            await channel.ExchangeDeclareAsync(
                exchange: context.Exchange,
                type: context.ExchangeType,
                durable: true,
                autoDelete: false
            );

            var properties = context.BasicProperties ?? new BasicProperties
            {
                Persistent = context.Persistent,
                Priority = context.Priority
            };

            // Queue (only when specified)
            if (!string.IsNullOrWhiteSpace(context.Queue))
            {
                await channel.QueueDeclareAsync(
                    queue: context.Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: context.Arguments
                );

                await channel.QueueBindAsync(
                    queue: context.Queue,
                    exchange: context.Exchange,
                    routingKey: context.RoutingKey
                );
            }

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await channel.BasicPublishAsync(
                exchange: context.Exchange,
                routingKey: context.RoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: body
            );
        }
    }
}
