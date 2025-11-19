using AccountingOffice.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingOffice.Infrastructure.Configuration.DependencyInjection;

/// <summary>
/// Extensões principais para registro de todos os serviços da camada Infrastructure
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Registra todos os serviços da camada Infrastructure
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registrar DbContext
        services.AddDatabaseContext(configuration);

        // Registrar Repositórios
        services.AddRepositories();

        // Registrar Query Services
        services.AddQueryServices();

        // Registrar serviços da Application (CQRS)
        services.AddApplicationServices();

        // Registrar handlers de eventos
        services.AddEventHandlers();

        return services;
    }
}
