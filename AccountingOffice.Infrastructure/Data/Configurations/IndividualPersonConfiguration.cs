using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountingOffice.Infrastructure.Data.Configurations;

public class IndividualPersonConfiguration : IEntityTypeConfiguration<IndividualPerson>
{
    public void Configure(EntityTypeBuilder<IndividualPerson> builder)
    {
        builder.ToTable("IndividualPersons");

        builder.HasKey(ip => ip.Id);

        builder.Property(ip => ip.Id)
            .ValueGeneratedNever();

        builder.Property(ip => ip.TenantId)
            .IsRequired();

        builder.Property(ip => ip.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ip => ip.Document)
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(ip => new { ip.TenantId, ip.Document })
            .IsUnique();

        builder.Property(ip => ip.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ip => ip.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ip => ip.Phone)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(ip => ip.Active)
            .IsRequired();

        builder.Property(ip => ip.CreatedAt)
            .IsRequired();

        builder.Property(ip => ip.MaritalStatus)
            .IsRequired()
            .HasConversion<int>();
    }
}
