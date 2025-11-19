using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountingOffice.Infrastructure.Data.Configurations;

public class AccountReceivableConfiguration : IEntityTypeConfiguration<AccountReceivable>
{
    public void Configure(EntityTypeBuilder<AccountReceivable> builder)
    {
        builder.ToTable("AccountsReceivable");

        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.Id)
            .ValueGeneratedNever();

        builder.Property(ar => ar.TenantId)
            .IsRequired();

        builder.HasIndex(ar => ar.TenantId);

        builder.Property(ar => ar.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ar => ar.Ammount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ar => ar.IssueDate)
            .IsRequired();

        builder.Property(ar => ar.DueDate)
            .IsRequired();

        builder.Property(ar => ar.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ar => ar.PayMethod)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ar => ar.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(ar => new { ar.TenantId, ar.InvoiceNumber })
            .IsUnique();

        builder.Property(ar => ar.ReceivedDate);

        builder.HasOne(ar => ar.RelatedParty)
            .WithMany()
            .HasForeignKey("RelatedPartyId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(ar => ar.Installments, installment =>
        {
            installment.ToTable("InstallmentsReceivable");

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
