// EXEMPLO DE USO NO COMMAND HANDLER
// Não compile este arquivo - é apenas referência

/*
using AccountingOffice.Application.Events;
using AccountingOffice.Infrastructure.MessageBroker;

public class AccountPayableCommandHandler
{
    private readonly IEventPublisherService _eventPublisher;
    
    public AccountPayableCommandHandler(IEventPublisherService eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }
    
    public async Task<Result<Guid>> Handle(CreateAccountPayableCommand command, CancellationToken cancellationToken)
    {
        // 1. Criar a conta normalmente
        var account = AccountPayable.Create(...);
        await _repository.CreateAsync(account);
        
        // 2. Publicar evento ASSÍNCRONO para consolidado
        // ⚠️ Mesmo que o consolidado esteja offline, a mensagem fica na fila
        var @event = new AccountPayableCreatedEvent(
            account.Id,
            account.TenantId,
            account.Ammount,
            account.IssueDate,
            account.DueDate,
            account.Description
        );
        
        // Fire-and-forget: não bloqueia o lançamento
        await _eventPublisher.PublishConsolidationEventAsync(@event, cancellationToken);
        
        return Result<Guid>.Success(account.Id);
    }
}
*/

// CONFIGURAÇÃO NO appsettings.json:
/*
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "ConsolidationExchange": "accounting.consolidation",
    "DailyConsolidationQueue": "accounting.consolidation.daily",
    "MaxRetryAttempts": 3,
    "MessageTTL": 86400000,
    "PrefetchCount": 50
  }
}
*/

// CONFIGURAÇÃO NO Program.cs ou Startup:
/*
// Adicionar Message Broker
builder.Services.AddMessageBroker(builder.Configuration);
*/
