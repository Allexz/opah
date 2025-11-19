using AccountingOffice.Domain.Core.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountingOffice.Infrastructure.Data.Configurations;

public class LegalPersonConfiguration : IEntityTypeConfiguration<LegalPerson>
{
    public void Configure(EntityTypeBuilder<LegalPerson> builder)
    {
        // A configuração TPH já está definida em IndividualPersonConfiguration
        // Aqui apenas configuramos as propriedades específicas de LegalPerson
        
        // Propriedade específica de LegalPerson
        builder.Property(lp => lp.LegalName)
            .IsRequired()
            .HasMaxLength(250);
    }
}
