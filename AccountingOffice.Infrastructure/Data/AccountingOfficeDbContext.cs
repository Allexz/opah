using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AccountingOffice.Infrastructure.Data;

public class AccountingOfficeDbContext : DbContext
{
    public AccountingOfficeDbContext(DbContextOptions<AccountingOfficeDbContext> options)
        : base(options)
    {
    }

    // Construtor protegido para testes e cenários específicos
    protected AccountingOfficeDbContext()
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Person<Guid>> Persons { get; set; }
   
    public DbSet<AccountPayable> AccountsPayable { get; set; }
    public DbSet<AccountReceivable> AccountsReceivable { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
           warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações do assembly automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountingOfficeDbContext).Assembly);

        // Configurações globais
        ConfigureGlobalSettings(modelBuilder);

        // Configuração específica para SQL Server
        if (Database.IsSqlServer())
        {
            ConfigureSqlServer(modelBuilder);
        }

        // Dados iniciais - Company
        var companyResult = Company.Create(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Microworkers do Brasil",
            "48.245.009/0001-99",
            "cia@microworkes.com.br",
            "(27)90004-5444",
            true);
        
        if (companyResult.IsSuccess)
        {
            modelBuilder.Entity<Company>().HasData(companyResult.Value);
        }

        // Dados iniciais - User
        var userResult = User.Create(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Alexandre",
            "Abcd1234****");
            
        if (userResult.IsSuccess)
        {
            // Precisamos criar um objeto anônimo com as propriedades que podem ser definidas
            modelBuilder.Entity<User>().HasData(new
            {
                Id = 1,
                TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                UserName = "Alexandre",
                Password = "Abcd1234****",
                Active = true,
                CreatedAt = new DateTime(2025, 1, 1)
            });
        }
    }

    private void ConfigureGlobalSettings(ModelBuilder modelBuilder)
    {
        // Desabilitar cascade delete globalmente para maior controle
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Configurar precisão decimal padrão para todas as propriedades decimais
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        // Configurar tamanho máximo padrão para strings
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(string) && p.GetMaxLength() == null))
        {
            property.SetMaxLength(500);
        }
    }

    private void ConfigureSqlServer(ModelBuilder modelBuilder)
    {
        // Configurações específicas do SQL Server
        modelBuilder.HasDefaultSchema("dbo");

        // Configurar collation padrão para SQL Server (case-insensitive)
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(string)))
        {
            property.SetCollation("SQL_Latin1_General_CP1_CI_AS");
        }
    }

    //private void ConfigurePostgreSQL(ModelBuilder modelBuilder)
    //{
    //    // Configurações específicas do PostgreSQL
    //    // PostgreSQL é case-sensitive, então podemos configurar nomes em lowercase
        
    //    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    //    {
    //        // Converter nomes de tabelas para lowercase
    //        entity.SetTableName(entity.GetTableName()?.ToLower());

    //        // Converter nomes de colunas para lowercase
    //        foreach (var property in entity.GetProperties())
    //        {
    //            property.SetColumnName(property.GetColumnName().ToLower());
    //        }

    //        // Converter nomes de chaves para lowercase
    //        foreach (var key in entity.GetKeys())
    //        {
    //            key.SetName(key.GetName()?.ToLower());
    //        }

    //        // Converter nomes de foreign keys para lowercase
    //        foreach (var fk in entity.GetForeignKeys())
    //        {
    //            fk.SetConstraintName(fk.GetConstraintName()?.ToLower());
    //        }

    //        // Converter nomes de índices para lowercase
    //        foreach (var index in entity.GetIndexes())
    //        {
    //            index.SetDatabaseName(index.GetDatabaseName()?.ToLower());
    //        }
    //    }
    //}

    public override int SaveChanges()
    {
        OnBeforeSaving();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void OnBeforeSaving()
    {
        var entries = ChangeTracker.Entries();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            // Atualizar timestamps automáticos se a entidade tiver CreatedAt
            if (entry.State == EntityState.Added)
            {
                var createdAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
                if (createdAtProperty != null && createdAtProperty.Metadata.ClrType == typeof(DateTime))
                {
                    createdAtProperty.CurrentValue = now;
                }
            }

            // Garantir que campos de texto sejam trimmed
            foreach (var property in entry.Properties.Where(p => p.Metadata.ClrType == typeof(string)))
            {
                if (property.CurrentValue is string value && !string.IsNullOrEmpty(value))
                {
                    property.CurrentValue = value.Trim();
                }
            }
        }
    }

    /// <summary>
    /// Inicia uma transação de banco de dados
    /// </summary>
    public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Executa migrations pendentes
    /// </summary>
    public async Task MigrateDatabaseAsync()
    {
        await Database.MigrateAsync();
    }

    /// <summary>
    /// Verifica se o banco de dados pode ser conectado
    /// </summary>
    public async Task<bool> CanConnectAsync()
    {
        return await Database.CanConnectAsync();
    }
}
