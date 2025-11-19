using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.AccountPay.Queries;
using AccountingOffice.Application.UseCases.AccountPay.Queries.Result;
using AccountingOffice.Application.UseCases.AccountPay.QueryHandler;
using AccountingOffice.Application.UseCases.AccountReceiv.Queries;
using AccountingOffice.Application.UseCases.AccountReceiv.Queries.Result;
using AccountingOffice.Application.UseCases.AccountReceiv.QueryHandler;
using AccountingOffice.Application.UseCases.Cia.Queries;
using AccountingOffice.Application.UseCases.Cia.Queries.Result;
using AccountingOffice.Application.UseCases.Cia.QueryHandler;
using AccountingOffice.Application.UseCases.Consolidation.Queries;
using AccountingOffice.Application.UseCases.Consolidation.Queries.Result;
using AccountingOffice.Application.UseCases.Consolidation.QueryHandler;
using AccountingOffice.Application.UseCases.Individual.Queries;
using AccountingOffice.Application.UseCases.Individual.Queries.Result;
using AccountingOffice.Application.UseCases.Individual.QueryHandler;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingOffice.Infrastructure.Configuration.DependencyInjection;

/// <summary>
/// Extensões para registro de Query Handlers
/// </summary>
public static class QueryHandlerExtensions
{
    /// <summary>
    /// Registra todos os Query Handlers da aplicação
    /// </summary>
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        // AccountPayable Query Handlers
        services.AddAccountPayableQueryHandlers();
        
        // AccountReceivable Query Handlers
        services.AddAccountReceivableQueryHandlers();
        
        // Company Query Handlers
        services.AddCompanyQueryHandlers();
        
        // IndividualPerson Query Handlers
        services.AddIndividualPersonQueryHandlers();
        
        // Consolidation Query Handlers
        services.AddConsolidationQueryHandlers();

        return services;
    }

    /// <summary>
    /// Registra Query Handlers de AccountPayable
    /// </summary>
    private static IServiceCollection AddAccountPayableQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetAccountPayByIdQuery, Result<AccountPayableResult?>>, AccountPayableQueryHandler>();
        services.AddScoped<IQueryHandler<GetAccountPayByTenantIdQuery, Result<IEnumerable<AccountPayableResult>>>, AccountPayableQueryHandler>();
        services.AddScoped<IQueryHandler<GetAccountPayByIssueDateQuery, Result<IEnumerable<AccountPayableResult>>>, AccountPayableQueryHandler>();
        services.AddScoped<IQueryHandler<GetAccountPayByRelatedPartQuery, Result<IEnumerable<AccountPayableResult>>>, AccountPayableQueryHandler>();

        return services;
    }

    /// <summary>
    /// Registra Query Handlers de AccountReceivable
    /// </summary>
    private static IServiceCollection AddAccountReceivableQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetAccountReceivByIdQuery, Result<AccountReceivableResult?>>, AccountReceivableQueryHandler>();
        services.AddScoped<IQueryHandler<GetAccountReceivByTenantId, Result<IEnumerable<AccountReceivableResult>>>, AccountReceivableQueryHandler>();
        services.AddScoped<IQueryHandler<GetAccountReceivByIssueDateQuery, Result<IEnumerable<AccountReceivableResult>>>, AccountReceivableQueryHandler>();
        services.AddScoped<IQueryHandler<GetAccountReceivByRelatedPartyQuery, Result<IEnumerable<AccountReceivableResult>>>, AccountReceivableQueryHandler>();

        return services;
    }

    /// <summary>
    /// Registra Query Handlers de Company
    /// </summary>
    private static IServiceCollection AddCompanyQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetCompanyByIdQuery, Result<CompanyResult?>>, CompanyQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllCompaniesQuery, Result<IEnumerable<CompanyResult?>>>, CompanyQueryHandler>();
        services.AddScoped<IQueryHandler<GetCompanyByDocumentQuery, Result<CompanyResult?>>, CompanyQueryHandler>();

        return services;
    }

    /// <summary>
    /// Registra Query Handlers de IndividualPerson
    /// </summary>
    private static IServiceCollection AddIndividualPersonQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetIndividualByIdQuery, Result<IndividualPersonResult?>>, IndividualQueryHandler>();
        services.AddScoped<IQueryHandler<GetIndividualByTenantId, Result<IEnumerable<IndividualPersonResult>>>, IndividualQueryHandler>();
        services.AddScoped<IQueryHandler<GetIndividualByDocument, Result<IndividualPersonResult?>>, IndividualQueryHandler>();

        return services;
    }

    /// <summary>
    /// Registra Query Handlers de Consolidation
    /// </summary>
    private static IServiceCollection AddConsolidationQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetDailyConsolidationQuery, Result<DailyConsolidationResult?>>, DailyConsolidationQueryHandler>();

        return services;
    }
}
