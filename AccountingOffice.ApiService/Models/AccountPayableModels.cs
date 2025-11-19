namespace AccountingOffice.ApiService.Models;

public record AccountPayableCreation(Guid TenantId,
                                     Guid SupplierId,
                                     string Description,
                                     decimal Amount,
                                     DateTime DueDate,
                                     int Status,
                                     int PayMethod,
                                     DateTime? PaymentDate);

public record AccountPayableUpdate(Guid TenantId,
                                   Guid AccountId,
                                   string? Description,
                                   int? Status,
                                   int? PayMethod,
                                   DateTime? PaymentDate);

public record AccountPayableListFilter(Guid TenantId,
                                       int PageNumber = 1,
                                       int PageSize = 20);

public record AccountPayableRelatedPartyFilter(Guid TenantId,
                                               Guid RelatedPartyId,
                                               int PageNumber = 1,
                                               int PageSize = 20);

public record AccountPayableDateRangeFilter(Guid TenantId,
                                            DateTime StartDate,
                                            DateTime EndDate,
                                            int PageNumber = 1,
                                            int PageSize = 20);

public record AccountPayableView(Guid Id,
                                 Guid TenantId,
                                 string Description,
                                 decimal Amount,
                                 DateTime IssueDate,
                                 DateTime DueDate,
                                 int Status,
                                 PersonView RelatedParty,
                                 int PayMethod,
                                 DateTime? PaymentDate);




