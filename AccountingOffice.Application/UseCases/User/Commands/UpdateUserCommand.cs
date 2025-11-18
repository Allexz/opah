using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Usr.Commands;

public sealed record UpdateUserCommand : ICommand<Result<bool>>
{
    public UpdateUserCommand(int id, Guid TenantId, string userName, string password)
    {
        Id = id;
        UserName = userName;
        Password = password;
    }
    public bool HasUserName => !string.IsNullOrWhiteSpace(UserName);
    public bool HasPasswor => !string.IsNullOrWhiteSpace(Password);
    public int Id { get; set; }
    public string UserName { get; init; }
    public string Password { get; init; }
    public Guid TenantId { get; init; }

}
