using System.Net;
using AccountingOffice.ApiService.Models;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Usr.Commands;
using Microsoft.AspNetCore.Mvc;

namespace AccountingOffice.ApiService.Features;

[Route("services/[controller]")]
[ApiController]
public sealed class UsersController : ApiControllerBase
{
    public UsersController(IApplicationBus applicationBus) : base(applicationBus)
    {
    }

    /// <summary>
    /// Criar usuário
    /// </summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] UserCreation payload, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(payload.TenantId, payload.UserName, payload.Password);
        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.Created);
    }

    /// <summary>
    /// Atualizar usuário
    /// </summary>
    [HttpPut("{id:int}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id,
                                                 [FromBody] UserUpdate payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(id,
                                            payload.TenantId,
                                            payload.UserName ?? string.Empty,
                                            payload.Password ?? string.Empty);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Excluir usuário
    /// </summary>
    [HttpDelete("{id:int}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id,
                                                 [FromQuery] Guid tenantId,
                                                 CancellationToken cancellationToken)
    {
        var command = new DeleteUserComands(id, tenantId);
        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Atualizar status do usuário
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ToggleStatusAsync([FromRoute] int id,
                                                       [FromBody] UserToggle payload,
                                                       CancellationToken cancellationToken)
    {
        var command = new ToggleUserActiveStatusCommand(id, payload.TenantId, payload.Active);
        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }
}

