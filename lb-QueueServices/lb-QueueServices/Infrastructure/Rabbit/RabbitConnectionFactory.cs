using lb_QueueServices.Domain.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace lb_QueueServices.Infrastructure.Rabbit
{
    public sealed class RabbitConnectionFactory
    {
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