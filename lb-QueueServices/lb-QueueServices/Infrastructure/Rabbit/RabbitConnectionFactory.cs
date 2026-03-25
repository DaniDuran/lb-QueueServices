using lb_QueueServices.Domain.Models;
using RabbitMQ.Client;

namespace lb_QueueServices.Infrastructure.Rabbit
{
    /// <summary>
    /// Factory for creating RabbitMQ connections using a <see cref="QueueContext"/>.
    /// </summary>
    public sealed class RabbitConnectionFactory
    {
        /// <summary>
        /// Creates a new connection using the provided context configuration.
        /// </summary>
        /// <param name="context">Connection settings.</param>
        /// <returns>An open RabbitMQ connection.</returns>
        public async Task<IConnection> CreateConnectionAsync(
            QueueContext context)
        {
            var factory = new ConnectionFactory
            {
                HostName = context.Host,
                Port = context.Port,
                UserName = context.User,
                Password = context.Password
                //VirtualHost = context.VirtualHost
            };

            return await factory.CreateConnectionAsync();
        }
    }
}
