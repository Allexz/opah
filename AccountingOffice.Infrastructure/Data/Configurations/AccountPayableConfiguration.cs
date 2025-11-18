using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountingOffice.Infrastructure.Data.Configurations;

public class AccountPayableConfiguration : IEntityTypeConfiguration<AccountPayable>
{
    public void Configure(EntityTypeBuilder<AccountPayable> builder)
    {
        builder.ToTable("AccountsPayable");

        builder.HasKey(ap => ap.Id);

        builder.Property(ap => ap.Id)
            .ValueGeneratedNever();

        builder.Property(ap => ap.TenantId)
            .IsRequired();

        builder.HasIndex(ap => ap.TenantId);

        builder.Property(ap => ap.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ap => ap.Ammount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ap => ap.IssueDate)
            .IsRequired();

        builder.Property(ap => ap.DueDate)
            .IsRequired();

        builder.Property(ap => ap.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ap => ap.PayMethod)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ap => ap.PaymentDate);

        builder.HasOne(ap => ap.RelatedParty)
            .WithMany()
            .HasForeignKey("RelatedPartyId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(ap => ap.Installments, installment =>
        {
            installment.ToTable("Installments");

            installment.WithOwner()
                .HasForeignKey("AccountId");

            installment.Property<Guid>("AccountId")
                .IsRequired();

            installment.HasKey("AccountId", nameof(Installment.InstallmentNumber));

            installment.Property(i => i.InstallmentNumber)
                .IsRequired();

            installment.Property(i => i.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            installment.Property(i => i.DueDate)
                .IsRequired();

            installment.Property(i => i.Status)
                .IsRequired()
                .HasConversion<int>();

            installment.Property(i => i.PaymentDate);

            installment.Property(i => i.Entrytype)
                .IsRequired()
                .HasConversion<int>();

            installment.Ignore(i => i.IsPaid);
            installment.Ignore(i => i.IsOverdue);
        });
    }
}
