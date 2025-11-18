using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountingOffice.Infrastructure.Data.Configurations;

public class LegalPersonConfiguration : IEntityTypeConfiguration<LegalPerson>
{
    public void Configure(EntityTypeBuilder<LegalPerson> builder)
    {
        builder.ToTable("LegalPersons");

        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.Id)
            .ValueGeneratedNever();

        builder.Property(lp => lp.TenantId)
            .IsRequired();

        builder.Property(lp => lp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(lp => lp.Document)
            .IsRequired()
            .HasMaxLength(18);

        builder.HasIndex(lp => new { lp.TenantId, lp.Document })
            .IsUnique();

        builder.Property(lp => lp.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(lp => lp.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(lp => lp.Phone)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(lp => lp.Active)
            .IsRequired();

        builder.Property(lp => lp.CreatedAt)
            .IsRequired();

        builder.Property(lp => lp.LegalName)
            .IsRequired()
            .HasMaxLength(250);
    }
}
