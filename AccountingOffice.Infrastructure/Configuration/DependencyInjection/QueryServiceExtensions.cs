using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Infrastructure.Data.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingOffice.Infrastructure.Configuration.DependencyInjection;

/// <summary>
/// Extensões para registro de Query Services
/// </summary>
public static class QueryServiceExtensions
{
    /// <summary>
    /// Registra todos os serviços de Query da aplicação
    /// </summary>
    public static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        // Nota: As implementações concretas dos Query Services devem ser criadas
        // na camada Infrastructure/Data/Queries

        // Quando as implementações estiverem prontas, descomente e ajuste:

        services.AddScoped<IAccountPayableQuery, AccountPayableQuery>();
        services.AddScoped<IAccountReceivableQuery, AccountReceivableQuery>();
        services.AddScoped<ICompanyQuery, CompanyQuery>();
        services.AddScoped<IIndividualPersonQuery, IndividualPersonQuery>();
        services.AddScoped<ILegalPersonQuery, LegalPersonQuery>();
        services.AddScoped<IPersonQuery, PersonQuery>();
        services.AddScoped<IUserQuery, UserQuery>();

        return services;
    }
}
