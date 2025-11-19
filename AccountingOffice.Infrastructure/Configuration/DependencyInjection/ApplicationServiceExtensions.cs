using AccountingOffice.Application.Infrastructure.ServicesBus.Behaviors;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingOffice.Infrastructure.Configuration.DependencyInjection;

/// <summary>
/// Extensões para registro de serviços da camada Application
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Registra o Application Bus (CQRS)
    /// </summary>
    public static IServiceCollection AddApplicationBus(this IServiceCollection services)
    {
        services.AddScoped<IApplicationBus, ApplicationBus>();
        return services;
    }

    /// <summary>
    /// Registra todos os serviços da camada Application
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddApplicationBus();
        services.AddCommandHandlers();
        services.AddQueryHandlers();
        
        return services;
    }
}
