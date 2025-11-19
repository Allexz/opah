namespace AccountingOffice.ApiService.Models;

public record AccountReceivableCreation(Guid TenantId,
                                        Guid CustomerId,
                                        string Description,
                                        decimal Amount,
                                        DateTime DueDate,
                                        DateTime IssueDate,
                                        int PayMethod,
                                        string InvoiceNumber);

public record AccountReceivableUpdate(Guid TenantId,
                                      Guid AccountId,
                                      string? Description,
                                      DateTime? DueDate,
                                      int? PayMethod);

public record AccountReceivableListFilter(Guid TenantId,
                                          int PageNumber = 1,
                                          int PageSize = 20);

public record AccountReceivableRelatedPartyFilter(Guid TenantId,
                                                  Guid RelatedPartyId,
                                                  int PageNumber = 1,
                                                  int PageSize = 20);

public record AccountReceivableDateRangeFilter(Guid TenantId,
                                               DateTime StartDate,
                                               DateTime EndDate,
                                               int PageNumber = 1,
                                               int PageSize = 20);

public record AccountReceivableView(Guid Id,
                                    Guid TenantId,
                                    string Description,
                                    decimal Amount,
                                    DateTime IssueDate,
                                    DateTime DueDate,
                                    int Status,
                                    PersonView Customer,
                                    int PayMethod,
                                    string InvoiceNumber,
                                    DateTime? ReceivedDate);

