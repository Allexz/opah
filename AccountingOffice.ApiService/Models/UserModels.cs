namespace AccountingOffice.ApiService.Models;

public record UserCreation(Guid TenantId,
                           string UserName,
                           string Password);

public record UserUpdate(Guid TenantId,
                         int UserId,
                         string? UserName,
                         string? Password);

public record UserToggle(Guid TenantId,
                         int UserId,
                         bool Active);




