using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;

namespace AccountingOffice.Application.UseCases.Usr.Commands;

public sealed record DeleteUserComands(int Id, Guid Tenant) : ICommand<Result<bool>>;
 
