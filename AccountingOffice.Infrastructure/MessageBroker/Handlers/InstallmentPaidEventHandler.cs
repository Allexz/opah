using AccountingOffice.Application.Events;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Infrastructure.MessageBroker;
using Microsoft.Extensions.Logging;

namespace AccountingOffice.Infrastructure.MessageBroker.Handlers;

/// <summary>
/// Handler para processar eventos de pagamento de parcelas
/// </summary>
public class InstallmentPaidEventHandler : IEventHandler<InstallmentPaidEvent>
{
    private readonly IEventPublisherService _eventPublisher;
    private readonly ILogger<InstallmentPaidEventHandler> _logger;

    public InstallmentPaidEventHandler(
        IEventPublisherService eventPublisher,
        ILogger<InstallmentPaidEventHandler> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task Handle(InstallmentPaidEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            // Publicar o evento no RabbitMQ para o serviço de consolidado
            await _eventPublisher.PublishConsolidationEventAsync(@event, cancellationToken);
            
            _logger.LogInformation(
                "Evento de parcela paga publicado para consolidação: AccountId={AccountId}, InstallmentNumber={InstallmentNumber}",
                @event.AccountId,
                @event.InstallmentNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Falha ao publicar evento de parcela paga: AccountId={AccountId}, InstallmentNumber={InstallmentNumber}",
                @event.AccountId,
                @event.InstallmentNumber);
            
            // Relançar a exceção para que o ApplicationBus possa tratá-la adequadamente
            throw;
        }
    }
}