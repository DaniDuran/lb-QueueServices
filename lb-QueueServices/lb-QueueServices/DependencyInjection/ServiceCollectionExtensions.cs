using lb_QueueServices.Domain.Contracts;
using lb_QueueServices.Infrastructure.Rabbit;
using Microsoft.Extensions.DependencyInjection;

namespace lb_QueueServices.DependencyInjection
{
    /// <summary>
    /// Dependency injection helpers for registering queue services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the queue publisher and consumer services using scoped lifetimes.
        /// </summary>
        /// <param name="services">The service collection to register into.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
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
