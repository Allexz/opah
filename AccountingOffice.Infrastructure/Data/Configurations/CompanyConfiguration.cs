using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountingOffice.Infrastructure.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Document)
            .IsRequired()
            .HasMaxLength(18);

        builder.HasIndex(c => c.Document)
            .IsUnique();

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Phone)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(c => c.Active)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();
    }
}
