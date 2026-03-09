using lb_QueueServices.Domain.Contracts;
using lb_QueueServices.Infrastructure.Rabbit;
using Microsoft.Extensions.DependencyInjection;

namespace lb_QueueServices.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
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
