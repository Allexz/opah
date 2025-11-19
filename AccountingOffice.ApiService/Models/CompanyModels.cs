namespace AccountingOffice.ApiService.Models;

public record CreateCompanyPayload(string Name,
                                   string Document,
                                   string Email,
                                   string Phone,
                                   bool Active);

public record UpdateCompanyPayload(string? Name,
                                   string? Email,
                                   string? Phone);

public record ToggleCompanyActivePayload(bool Active);

public record CompanyCollectionFilter(int PageNumber = 1,
                                      int PageSize = 20);

public record CompanyView(Guid Id,
                          string Name,
                          string Document,
                          string Email,
                          string Phone,
                          bool Active,
                          DateTime CreatedAt);




