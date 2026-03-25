using lb_QueueServices.Domain.Models;
using RabbitMQ.Client;

namespace lb_QueueServices.Infrastructure.Rabbit
{
    /// <summary>
    /// Fabrica de conexiones RabbitMQ basada en <see cref="QueueContext"/>.
    /// </summary>
    public sealed class RabbitConnectionFactory
    {
        /// <summary>
        /// Crea una nueva conexion usando la configuracion del contexto.
        /// </summary>
        /// <param name="context">Parametros de conexion.</param>
        /// <returns>Conexion RabbitMQ abierta.</returns>
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
