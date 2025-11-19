﻿﻿using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.Interfaces.Queries;
using AccountingOffice.Application.Interfaces.Repositories;
using AccountingOffice.Application.UseCases.Usr.Commands;
using AccountingOffice.Domain.Core.Aggregates;
using AccountingOffice.Domain.Core.Common;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<UserCommandHandler> _logger;

    public UserCommandHandler(
        IUserRepository userRepository, 
        IUserQuery userQuery,
        IApplicationBus applicationBus,
        ILogger<UserCommandHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userQuery = userQuery ?? throw new ArgumentNullException(nameof(userQuery));
        _applicationBus = applicationBus ?? throw new ArgumentNullException(nameof(applicationBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Result<int>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando criação de usuário. TenantId: {TenantId}, UserName: {UserName}",
            command.TenantId, command.UserName);

        DomainResult<User> result = User.Create(
            command.TenantId,
            command.UserName,
            command.Password);

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Falha na criação de usuário. TenantId: {TenantId}, UserName: {UserName}, Error: {Error}",
                command.TenantId, command.UserName, result.Error);
            return Result<int>.Failure(result.Error);
        }

        await _userRepository.CreateAsync(result.Value);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Usuário criado com sucesso. UserId: {UserId}, TenantId: {TenantId}, UserName: {UserName}, DurationMs: {DurationMs}",
            result.Value.Id, command.TenantId, command.UserName, stopwatch.ElapsedMilliseconds);
        
        // Publicar evento genérico para usuários
        // Como não há evento específico para usuário, usamos um evento genérico
        var @event = new object(); // Placeholder para evento futuro
        
        return Result<int>.Success(result.Value.Id);
    }

    public async Task<Result<bool>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando atualização de usuário. UserId: {UserId}, TenantId: {TenantId}, HasUserName: {HasUserName}, HasPassword: {HasPassword}",
            command.Id, command.TenantId, command.HasUserName, command.HasPasswor);

        User? usr = await _userQuery.GetByIdAsync(command.Id, command.TenantId);
        if (usr == null)
        {
            _logger.LogWarning(
                "Usuário não encontrado para atualização. UserId: {UserId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
            return Result<bool>.Failure("User not found.");
        }
        if (command.HasUserName)
        {
            DomainResult result = usr.ChangeUserName(command.UserName);
            if (result.IsFailure)
            {
                _logger.LogWarning(
                    "Falha ao alterar nome de usuário. UserId: {UserId}, TenantId: {TenantId}, Error: {Error}",
                    command.Id, command.TenantId, result.Error);
                return Result<bool>.Failure(result.Error);
            }
        }

        if (command.HasPasswor)
        {
            DomainResult result = usr.ChangePassword(command.Password);
            if (result.IsFailure)
            {
                _logger.LogWarning(
                    "Falha ao alterar senha de usuário. UserId: {UserId}, TenantId: {TenantId}, Error: {Error}",
                    command.Id, command.TenantId, result.Error);
                return Result<bool>.Failure(result.Error);
            }
        }

        await _userRepository.UpdateAsync(usr);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Usuário atualizado com sucesso. UserId: {UserId}, TenantId: {TenantId}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, stopwatch.ElapsedMilliseconds);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(DeleteUserComands command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando exclusão de usuário. UserId: {UserId}, TenantId: {TenantId}",
            command.Id, command.Tenant);

        User? usr = await _userQuery.GetByIdAsync(command.Id, command.Tenant);
        if (usr == null)
        {
            _logger.LogWarning(
                "Usuário não encontrado para exclusão. UserId: {UserId}, TenantId: {TenantId}",
                command.Id, command.Tenant);
            return Result<bool>.Failure("User not found.");
        }
        
        var userName = usr.UserName;
        await _userRepository.DeleteAsync(usr.Id);
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Usuário excluído com sucesso. UserId: {UserId}, TenantId: {TenantId}, UserName: {UserName}, DurationMs: {DurationMs}",
            command.Id, command.Tenant, userName, stopwatch.ElapsedMilliseconds);
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Handle(ToggleUserActiveStatusCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Iniciando alteração de status de usuário. UserId: {UserId}, TenantId: {TenantId}, Status: {Status}",
            command.Id, command.TenantId, command.status);

        User? usr = await _userQuery.GetByIdAsync(command.Id, command.TenantId);
        if (usr == null)
        {
            _logger.LogWarning(
                "Usuário não encontrado para alteração de status. UserId: {UserId}, TenantId: {TenantId}",
                command.Id, command.TenantId);
            return Result<bool>.Failure("User not found.");
        }
        
        var previousStatus = usr.Active;
        if (command.status)
            usr.Activate();
        else
            usr.Deactivate();
        stopwatch.Stop();

        _logger.LogInformation(
            "Status de usuário alterado com sucesso. UserId: {UserId}, TenantId: {TenantId}, PreviousStatus: {PreviousStatus}, NewStatus: {NewStatus}, DurationMs: {DurationMs}",
            command.Id, command.TenantId, previousStatus, usr.Active, stopwatch.ElapsedMilliseconds);

        return Result<bool>.Success(true);
    }
}
