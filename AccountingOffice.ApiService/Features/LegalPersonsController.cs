using System.Net;
using AccountingOffice.ApiService.Models;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Legal.Commands;
using Microsoft.AspNetCore.Mvc;

namespace AccountingOffice.ApiService.Features;

[Route("services/[controller]")]
[ApiController]
public sealed class LegalPersonsController : ApiControllerBase
{
    public LegalPersonsController(IApplicationBus applicationBus) : base(applicationBus)
    {
    }

    /// <summary>
    /// Criar pessoa jurídica
    /// </summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] LegalPersonCreation payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new CreateLegalPersonCommand(payload.TenantId,
                                                   payload.Name,
                                                   payload.Document,
                                                   payload.Email,
                                                   payload.PhoneNumber,
                                                   payload.LegalName);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.Created);
    }

    /// <summary>
    /// Atualizar pessoa jurídica
    /// </summary>
    [HttpPut("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id,
                                                 [FromBody] LegalPersonUpdate payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new UpdateLegalPersonCommand(payload.TenantId,
                                                   id,
                                                   payload.Name ?? string.Empty,
                                                   payload.Email ?? string.Empty,
                                                   payload.PhoneNumber ?? string.Empty,
                                                   payload.LegalName ?? string.Empty);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Excluir pessoa jurídica
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,
                                                 [FromQuery] Guid tenantId,
                                                 CancellationToken cancellationToken)
    {
        var command = new DeleteLegalPersonCommand(id, tenantId);
        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }
}

