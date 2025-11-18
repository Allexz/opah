using AccountingOffice.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingOffice.Infrastructure.Configuration.DependencyInjection;

/// <summary>
/// Extensões para registro de Repositórios
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Registra todos os repositórios da aplicação
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Nota: As implementações concretas dos repositórios devem ser criadas
        // na camada Infrastructure/Data/Repositories
        
        // Quando as implementações estiverem prontas, descomente e ajuste:
        
        // services.AddScoped<IAccountPayableRepository, AccountPayableRepository>();
        // services.AddScoped<IAccountReceivableRepository, AccountReceivableRepository>();
        // services.AddScoped<ICompanyRepository, CompanyRepository>();
        // services.AddScoped<IIndividualPersonRepository, IndividualPersonRepository>();
        // services.AddScoped<IInstalmentRepository, InstalmentRepository>();
        // services.AddScoped<ILegalPersonRepository, LegalPersonRepository>();
        // services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
