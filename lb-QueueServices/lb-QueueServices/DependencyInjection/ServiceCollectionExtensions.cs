using lb_QueueServices.Domain.Contracts;
using lb_QueueServices.Infrastructure.Rabbit;
using Microsoft.Extensions.DependencyInjection;

namespace lb_QueueServices.DependencyInjection
{
    /// <summary>
    /// Utilidades de inyeccion de dependencias para registrar servicios de colas.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra los servicios de consumo y publicacion con ciclo de vida scoped.
        /// </summary>
        /// <param name="services">Coleccion de servicios donde se registran.</param>
        /// <returns>La misma <see cref="IServiceCollection"/> para encadenar.</returns>
        public static IServiceCollection AddQueueServices(
            this IServiceCollection services)
        {
            services.AddScoped<IQueueConsumer, RabbitConsumer>();
            services.AddScoped<IQueueMonitor, RabbitConsumer>();
            services.AddScoped<IQueuePublisher, RabbitPublisher>();

            return services;
        }
    }
}
