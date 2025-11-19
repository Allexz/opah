using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using Microsoft.Extensions.Logging;

namespace AccountingOffice.Infrastructure.MessageBroker;

/// <summary>
/// Serviço para publicar eventos no sistema de consolidado
/// </summary>
public interface IEventPublisherService
{
    Task PublishConsolidationEventAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;
}

public class EventPublisherService : IEventPublisherService
{
    private readonly IRabbitMQPublisher _publisher;
    private readonly ILogger<EventPublisherService> _logger;

    public EventPublisherService(
        IRabbitMQPublisher publisher,
        ILogger<EventPublisherService> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task PublishConsolidationEventAsync<T>(T @event, CancellationToken cancellationToken = default) 
        where T : IEvent
    {
        try
        {
            var routingKey = GetRoutingKey(@event);
            
            await _publisher.PublishAsync(@event, routingKey, cancellationToken);
            
            _logger.LogInformation(
                "Evento de consolidação publicado: {EventType} com routing key {RoutingKey}",
                typeof(T).Name,
                routingKey);
        }
        catch (Exception ex)
        {
            // Log mas não falha - garante que o serviço de lançamento não pare
            _logger.LogError(
                ex,
                "Falha ao publicar evento de consolidação: {EventType}. Evento será retentado.",
                typeof(T).Name);
            
            // Opcional: Implementar fallback para Dead Letter Queue ou retry policy
            throw;
        }
    }

    private string GetRoutingKey<T>(T @event) where T : IEvent
    {
        var eventType = @event.GetType().Name;
        
        return eventType switch
        {
            "AccountPayableCreatedEvent" => "consolidation.account.payable.created",
            "AccountReceivableCreatedEvent" => "consolidation.account.receivable.created",
            "InstallmentPaidEvent" => "consolidation.installment.paid",
            "AccountUpdatedEvent" => "consolidation.account.updated",
            _ => "consolidation.generic"
        };
    }
}
