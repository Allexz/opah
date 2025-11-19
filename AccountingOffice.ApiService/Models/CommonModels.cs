namespace AccountingOffice.ApiService.Models;

public record PersonView(Guid Id,
                         Guid TenantId,
                         string Name,
                         string Document,
                         int PersonType,
                         string Email,
                         string Phone,
                         bool Active);



