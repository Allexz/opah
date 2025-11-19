using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountingOffice.Infrastructure.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person<Guid>>
{
    public void Configure(EntityTypeBuilder<Person<Guid>> builder)
    {
        // Configurar hierarquia TPH para Person - usar IndividualPerson como base
        builder.ToTable("Persons")
            .HasDiscriminator<PersonType>("PersonType")
            .HasValue<IndividualPerson>(PersonType.Individual)
            .HasValue<LegalPerson>(PersonType.Company);

        // Configurar chave primária
        builder.HasKey(p => p.Id);

        // Configurar propriedades comuns (herdadas de Person)
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Document)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(p => p.Active)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasIndex(p => new { p.TenantId, p.Document })
            .IsUnique();
    }
}
