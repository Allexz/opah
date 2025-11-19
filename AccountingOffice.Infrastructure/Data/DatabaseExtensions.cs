using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountingOffice.Infrastructure.Data;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseProvider = configuration["DatabaseProvider"] ?? "SqlServer";
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' não foi encontrada.");
        }

        services.AddDbContext<AccountingOfficeDbContext>(options =>
        {
            switch (databaseProvider.ToLower())
            {
                case "sqlserver":
                case "sql":
                    options.UseSqlServer(connectionString, sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        sqlOptions.MigrationsAssembly("AccountingOffice.Infrastructure");
                    });
                    break;


                //case "postgresql":
                //case "postgres":
                //case "npgsql":
                //    options.UseNpgsql(connectionString, npgsqlOptions =>
                //    {
                //        npgsqlOptions.EnableRetryOnFailure(
                //            maxRetryCount: 5,
                //            maxRetryDelay: TimeSpan.FromSeconds(30),
                //            errorCodesToAdd: null);
                //        npgsqlOptions.MigrationsAssembly("AccountingOffice.Infrastructure");
                //    });
                //    break;

                default:
                    throw new InvalidOperationException(
                        $"Provider de banco de dados '{databaseProvider}' não é suportado. " +
                        "Use 'SqlServer' ou 'PostgreSQL'.");
            }

            // Configurações comuns
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(true);
        });

        return services;
    }

    public static IApplicationBuilder UseApplicationDatabase(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AccountingOfficeDbContext>();
            context.Database.Migrate();
        }
        return app;

    }
}
