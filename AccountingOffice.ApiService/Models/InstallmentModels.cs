namespace AccountingOffice.ApiService.Models;

public record InstallmentCreation(Guid AccountId,
                                  Guid TenantId,
                                  int EntryType,
                                  int InstallmentNumber,
                                  decimal Amount,
                                  DateTime DueDate);

public record InstallmentStatusChange(Guid AccountId,
                                      Guid TenantId,
                                      int InstallmentNumber,
                                      int Status);

public record InstallmentDeletion(Guid AccountId,
                                  Guid TenantId,
                                  int InstallmentNumber,
                                  DateTime PaymentDate);

public record InstallmentPayment(Guid AccountId,
                                 Guid TenantId,
                                 int InstallmentNumber,
                                 DateTime PaymentDate,
                                 decimal PaymentAmount);

public record InstallmentAccountFilter(Guid AccountId,
                                       Guid TenantId,
                                       int PageNumber = 1,
                                       int PageSize = 20);

public record InstallmentTenantFilter(Guid TenantId,
                                      int PageNumber = 1,
                                      int PageSize = 20);

public record InstallmentView(int InstallmentNumber,
                              decimal Amount,
                              DateTime DueDate,
                              int Status,
                              DateTime? PaymentDate,
                              int EntryType);

