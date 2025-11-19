﻿﻿using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Usr.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;

namespace AccountingOffice.Application.UseCases.Usr.CommandHandler;

public class UserCommandHandler :
    ICommandHandler<CreateUserCommand, Result<int>>,
    ICommandHandler<UpdateUserCommand, Result<bool>>,
    ICommandHandler<DeleteUserComands, Result<bool>>  ,
    ICommandHandler<ToggleUserActiveStatusCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserQuery _userQuery;
    private readonly IApplicationBus _applicationBus;

    public UserCommandHandler(
        IUserRepository userRepository, 
        IUserQuery userQuery,
        IApplicationBus applicationBus)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userQuery = userQuery ?? throw new ArgumentNullException(nameof(userQuery));
        _applicationBus = applicationBus ?? throw new ArgumentNullException(nameof(applicationBus));
    }
    
    public async Task<Result<int>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        DomainResult<User> result = User.Create(
            command.TenantId,
            command.UserName,
            command.Password);

        if (result.IsFailure)
        {
            return Result<int>.Failure(result.Error);
        }

        await _userRepository.CreateAsync(result.Value);
        
        // Publicar evento genérico para usuários
        // Como não há evento específico para usuário, usamos um evento genérico
        var @event = new object(); // Placeholder para evento futuro
        
        return Result<int>.Success(result.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        User? usr = await _userQuery.GetByIdAsync(command.Id, command.TenantId);
        if (usr == null)
        {
            return Result<bool>.Failure("User not found.");
        }
        if (command.HasUserName)
        {
            DomainResult result = usr.ChangeUserName(command.UserName);
            if (result.IsFailure)
                return Result<bool>.Failure(result.Error);
        }

        if (command.HasPasswor)
        {
            DomainResult result = usr.ChangePassword(command.Password);
            if (result.IsFailure)
                return Result<bool>.Failure(result.Error);
        }

        await _userRepository.UpdateAsync(usr);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(DeleteUserComands command, CancellationToken cancellationToken)
    {
        User? usr = await _userQuery.GetByIdAsync(command.Id, command.Tenant);
        if (usr == null)
        {
            return Result<bool>.Failure("User not found.");
        }
        await _userRepository.DeleteAsync(usr.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(ToggleUserActiveStatusCommand command, CancellationToken cancellationToken)
    {
        User? usr = await _userQuery.GetByIdAsync(command.Id, command.TenantId);
        if (usr == null)
        {
            return Result<bool>.Failure("User not found.");
        }
        if (command.status)
            usr.Activate();
        else
            usr.Deactivate();

        return Result<bool>.Success(true);
    }
}
