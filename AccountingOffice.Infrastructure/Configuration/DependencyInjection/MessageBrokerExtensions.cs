using AccountingOffice.Infrastructure.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingOffice.Infrastructure.Configuration.DependencyInjection;

/// <summary>
/// Extensões para configuração do Message Broker (RabbitMQ)
/// </summary>
public static class MessageBrokerExtensions
{
    /// <summary>
    /// Adiciona os serviços de mensageria RabbitMQ
    /// </summary>
    public static IServiceCollection AddMessageBroker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar RabbitMQ
        services.Configure<RabbitMQConfiguration>(
            configuration.GetSection("RabbitMQ"));

        // Registrar Publisher como Singleton para reutilizar conexão
        services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

        // Registrar Event Publisher Service
        services.AddScoped<IEventPublisherService, EventPublisherService>();

        return services;
    }
}
