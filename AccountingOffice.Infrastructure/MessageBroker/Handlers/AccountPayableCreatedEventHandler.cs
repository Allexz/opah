using AccountingOffice.Application.Events;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Infrastructure.MessageBroker;
using Microsoft.Extensions.Logging;

namespace AccountingOffice.Infrastructure.MessageBroker.Handlers;

/// <summary>
/// Handler para processar eventos de criação de contas a pagar
/// </summary>
public class AccountPayableCreatedEventHandler : IEventHandler<AccountPayableCreatedEvent>
{
    private readonly IEventPublisherService _eventPublisher;
    private readonly ILogger<AccountPayableCreatedEventHandler> _logger;

    public AccountPayableCreatedEventHandler(
        IEventPublisherService eventPublisher,
        ILogger<AccountPayableCreatedEventHandler> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task Handle(AccountPayableCreatedEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            // Publicar o evento no RabbitMQ para o serviço de consolidado
            await _eventPublisher.PublishConsolidationEventAsync(@event, cancellationToken);
            
            _logger.LogInformation(
                "Evento de conta a pagar criada publicado para consolidação: AccountId={AccountId}, TenantId={TenantId}",
                @event.AccountId,
                @event.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Falha ao publicar evento de conta a pagar criada: AccountId={AccountId}, TenantId={TenantId}",
                @event.AccountId,
                @event.TenantId);
            
            // Relançar a exceção para que o ApplicationBus possa tratá-la adequadamente
            throw;
        }
    }
}