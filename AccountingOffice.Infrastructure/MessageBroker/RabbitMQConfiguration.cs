namespace AccountingOffice.Infrastructure.MessageBroker;

/// <summary>
/// Configurações do RabbitMQ
/// </summary>
public class RabbitMQConfiguration
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    
    /// <summary>
    /// Nome da exchange para eventos de consolidado
    /// </summary>
    public string ConsolidationExchange { get; set; } = "accounting.consolidation";
    
    /// <summary>
    /// Nome da fila para consolidado diário
    /// </summary>
    public string DailyConsolidationQueue { get; set; } = "accounting.consolidation.daily";
    
    /// <summary>
    /// Tentativas de retry antes de mover para DLQ
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;
    
    /// <summary>
    /// Tempo de TTL das mensagens (em milissegundos)
    /// </summary>
    public int MessageTTL { get; set; } = 86400000; // 24 horas
    
    /// <summary>
    /// Prefetch count para controle de throughput
    /// </summary>
    public ushort PrefetchCount { get; set; } = 50;
}
