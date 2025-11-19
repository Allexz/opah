namespace AccountingOffice.ApiService.Models;

public record IndividualCreation(Guid TenantId,
                                 string Name,
                                 string Document,
                                 int PersonType,
                                 string Email,
                                 string PhoneNumber,
                                 int MaritalStatus);

public record IndividualUpdate(Guid TenantId,
                               Guid PersonId,
                               string? Name,
                               string? Email,
                               string? PhoneNumber,
                               int? MaritalStatus);

public record IndividualView(Guid Id,
                             Guid TenantId,
                             string Name,
                             string Document,
                             int PersonType,
                             string Email,
                             string PhoneNumber,
                             int MaritalStatus);

public record IndividualCollectionFilter(Guid TenantId,
                                         int PageNumber = 1,
                                         int PageSize = 20);



