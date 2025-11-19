using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountingOffice.Infrastructure.Data.Configurations;

public class IndividualPersonConfiguration : IEntityTypeConfiguration<IndividualPerson>
{
    public void Configure(EntityTypeBuilder<IndividualPerson> builder)
    {
        // Propriedade específica de IndividualPerson
        builder.Property(ip => ip.MaritalStatus)
            .IsRequired()
            .HasConversion<int>();
    }
}
