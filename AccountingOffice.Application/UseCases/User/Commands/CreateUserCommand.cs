using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Usr.Commands;

public sealed record CreateUserCommand : ICommand<Result<int>>
{
    public CreateUserCommand(Guid tenantId,
                             string userName,
                             string password )
    {
        TenantId = tenantId;
        UserName = userName;
        Password = password;
        CreatedAt = DateTime.Now;
    }

    public Guid TenantId { get;init; }
    public string UserName { get; init; }
    public string Password { get; init; } 
    public DateTime CreatedAt { get;init; }
}
