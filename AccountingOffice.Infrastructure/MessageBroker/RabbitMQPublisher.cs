using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AccountingOffice.Infrastructure.MessageBroker;

/// <summary>
/// Publisher para enviar eventos ao RabbitMQ
/// </summary>
public interface IRabbitMQPublisher
{
    Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default);
}

public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly RabbitMQConfiguration _configuration;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private bool _disposed;

    public RabbitMQPublisher(
        IOptions<RabbitMQConfiguration> configuration,
        ILogger<RabbitMQPublisher> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _configuration.Host,
            Port = _configuration.Port,
            UserName = _configuration.Username,
            Password = _configuration.Password,
            VirtualHost = _configuration.VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        // Declarar exchange durável
        _channel.ExchangeDeclareAsync(
            exchange: _configuration.ConsolidationExchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false).GetAwaiter().GetResult();

        // Declarar fila durável com DLX
        var queueArgs = new Dictionary<string, object?>
        {
            { "x-message-ttl", _configuration.MessageTTL },
            { "x-dead-letter-exchange", $"{_configuration.ConsolidationExchange}.dlx" },
            { "x-max-length", 100000 } // Limitar tamanho da fila
        };

        _channel.QueueDeclareAsync(
            queue: _configuration.DailyConsolidationQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs).GetAwaiter().GetResult();

        // Bind queue to exchange
        _channel.QueueBindAsync(
            queue: _configuration.DailyConsolidationQueue,
            exchange: _configuration.ConsolidationExchange,
            routingKey: "consolidation.*").GetAwaiter().GetResult();

        _logger.LogInformation("RabbitMQ Publisher inicializado com sucesso");
    }

    public async Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true, // Mensagem durável
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent, // Persistente
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                MessageId = Guid.NewGuid().ToString(),
                Type = typeof(T).Name
            };

            await _channel.BasicPublishAsync(
                exchange: _configuration.ConsolidationExchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogDebug(
                "Mensagem publicada: {MessageType} com routing key {RoutingKey}",
                typeof(T).Name,
                routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem {MessageType}", typeof(T).Name);
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _channel?.CloseAsync().GetAwaiter().GetResult();
        _channel?.Dispose();
        _connection?.CloseAsync().GetAwaiter().GetResult();
        _connection?.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
