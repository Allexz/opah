namespace AccountingOffice.ApiService.Models;

public record LegalPersonCreation(Guid TenantId,
                                  string Name,
                                  string Document,
                                  string Email,
                                  string PhoneNumber,
                                  string LegalName);

public record LegalPersonUpdate(Guid TenantId,
                                string? Name,
                                string? Email,
                                string? PhoneNumber,
                                string? LegalName);

