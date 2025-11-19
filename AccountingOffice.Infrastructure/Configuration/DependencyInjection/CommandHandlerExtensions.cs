using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Account.CommandHandler;
using AccountingOffice.Application.UseCases.AccountPay.CommandHandler;
using AccountingOffice.Application.UseCases.AccountPay.Commands;
using AccountingOffice.Application.UseCases.AccountReceiv.CommandHandler;
using AccountingOffice.Application.UseCases.AccountReceiv.Commands;
using AccountingOffice.Application.UseCases.Cia.CommandHandler;
using AccountingOffice.Application.UseCases.Cia.Commands;
using AccountingOffice.Application.UseCases.Individual.CommandHandler;
using AccountingOffice.Application.UseCases.Individual.Commands;
using AccountingOffice.Application.UseCases.Installm.Commands;
using AccountingOffice.Application.UseCases.Installments.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingOffice.Infrastructure.Configuration.DependencyInjection;

/// <summary>
/// Extensões para registro de Command Handlers
/// </summary>
public static class CommandHandlerExtensions
{
    /// <summary>
    /// Registra todos os Command Handlers da aplicação
    /// </summary>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        // Account Command Handlers
        services.AddAccountCommandHandlers();
        
        // AccountPayable Command Handlers
        services.AddAccountPayableCommandHandlers();
        
        // AccountReceivable Command Handlers
        services.AddAccountReceivableCommandHandlers();
        
        // Company Command Handlers
        services.AddCompanyCommandHandlers();
        
        // IndividualPerson Command Handlers
        services.AddIndividualPersonCommandHandlers();

        return services;
    }

    /// <summary>
    /// Registra Command Handlers de Account (Installments)
    /// </summary>
    private static IServiceCollection AddAccountCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<AddInstallmentCommand, Result<bool>>, AccountCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteInstallmentCommand, Result<bool>>, AccountCommandHandler>();
        services.AddScoped<ICommandHandler<ChangeInstallmentStatusCommand, Result<bool>>, AccountCommandHandler>();

        return services;
    }

    /// <summary>
    /// Registra Command Handlers de AccountPayable
    /// </summary>
    private static IServiceCollection AddAccountPayableCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateAccountPayableCommand, Result<Guid>>, AccountPayableCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateAccountPayableCommand, Result<bool>>, AccountPayableCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteAccountPayableCommand, Result<bool>>, AccountPayableCommandHandler>();

        return services;
    }

    /// <summary>
    /// Registra Command Handlers de AccountReceivable
    /// </summary>
    private static IServiceCollection AddAccountReceivableCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateAccountReceivableCommand, Result<Guid>>, AccountReceivableCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateAccountReceivableCommand, Result<bool>>, AccountReceivableCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteAccountReceivableCommand, Result<bool>>, AccountReceivableCommandHandler>();

        return services;
    }

    /// <summary>
    /// Registra Command Handlers de Company
    /// </summary>
    private static IServiceCollection AddCompanyCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateCompanyCommand, Result<Guid>>, CompanyCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCompanyCommand, Result<bool>>, CompanyCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteCompanyCommand, Result<bool>>, CompanyCommandHandler>();
        services.AddScoped<ICommandHandler<ToggleCompanyActiveStatusCommand, Result<bool>>, CompanyCommandHandler>();

        return services;
    }

    /// <summary>
    /// Registra Command Handlers de IndividualPerson
    /// </summary>
    private static IServiceCollection AddIndividualPersonCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateIndividualPersonCommand, Result<Guid>>, IndividualPersonCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateIndividualPersonCommand, Result<bool>>, IndividualPersonCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteIndividualPersonCommand, Result<bool>>, IndividualPersonCommandHandler>();

        return services;
    }
}
