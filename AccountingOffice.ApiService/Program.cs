using AccountingOffice.ApiService.Middleware;
using AccountingOffice.Infrastructure.Configuration.DependencyInjection;
using AccountingOffice.Infrastructure.Data;
using AccountingOffice.Infrastructure.Logging;
using Serilog;
using System.Text.Json.Serialization;

// Configurar Serilog antes de criar o builder
var builder = WebApplication.CreateBuilder(args);

// Adicionar Logging com Serilog
builder.AddLoggingServices();

// Configuração de serviços
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Adicionar serviços de infraestrutura (DbContext, Repositories, Queries, CQRS)
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMessageBroker(builder.Configuration);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware de tratamento de exceções global (deve ser o primeiro)
app.UseGlobalExceptionHandler();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseApplicationDatabase();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar CORS
app.UseCors("AllowAll");

// Usar Serilog para logging de requisições
app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

// Migrar banco de dados automaticamente em desenvolvimento
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AccountingOfficeDbContext>();
    
    try
    {
        if (await dbContext.CanConnectAsync())
        {
            await dbContext.MigrateDatabaseAsync();
            Log.Information("Migrações de banco de dados aplicadas com sucesso");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Erro ao aplicar migrações de banco de dados");
    }
}

try
{
    Log.Information("Iniciando aplicação AccountingOffice API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
