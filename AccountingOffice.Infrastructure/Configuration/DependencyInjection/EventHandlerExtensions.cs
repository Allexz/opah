using AccountingOffice.Application.Events;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Infrastructure.MessageBroker;
using AccountingOffice.Infrastructure.MessageBroker.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingOffice.Infrastructure.Configuration.DependencyInjection;

/// <summary>
/// Extensões para registro de handlers de eventos
/// </summary>
public static class EventHandlerExtensions
{
    /// <summary>
    /// Registra todos os handlers de eventos do sistema de mensageria
    /// </summary>
    public static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        // Registrar handlers de eventos específicos
        services.AddScoped<IEventHandler<AccountPayableCreatedEvent>, AccountPayableCreatedEventHandler>();
        services.AddScoped<IEventHandler<AccountReceivableCreatedEvent>, AccountReceivableCreatedEventHandler>();
        services.AddScoped<IEventHandler<InstallmentPaidEvent>, InstallmentPaidEventHandler>();
        services.AddScoped<IEventHandler<AccountUpdatedEvent>, AccountUpdatedEventHandler>();
        services.AddScoped<IEventPublisherService, EventPublisherService>();

        return services;
    }
}