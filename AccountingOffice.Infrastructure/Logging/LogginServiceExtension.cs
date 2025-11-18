using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace AccountingOffice.Infrastructure.Logging;

public static class LogginServiceExtension
{
    /// <summary>
    /// Configura o Serilog para a aplicação com Elasticsearch
    /// </summary>
    public static WebApplicationBuilder AddLoggingServices(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticConfiguration:Uri"]!))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                IndexFormat = $"accountingoffice-api-{DateTime.UtcNow:yyyy-MM}",
                MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information
            })
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }
}
