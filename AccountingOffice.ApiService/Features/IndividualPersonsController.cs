using System.Net;
using AccountingOffice.ApiService.Models;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Individual.Commands;
using AccountingOffice.Application.UseCases.Individual.Queries;
using AccountingOffice.Application.UseCases.Individual.Queries.Result;
using Microsoft.AspNetCore.Mvc;

namespace AccountingOffice.ApiService.Features;

[Route("services/[controller]")]
[ApiController]
public sealed class IndividualPersonsController : ApiControllerBase
{
    public IndividualPersonsController(IApplicationBus applicationBus) : base(applicationBus)
    {
    }

    /// <summary>
    /// Criar pessoa física
    /// </summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] IndividualCreation payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new CreateIndividualPersonCommand(payload.TenantId,
                                                        payload.Name,
                                                        payload.Document,
                                                        payload.Email,
                                                        payload.PhoneNumber,
                                                        payload.MaritalStatus);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.Created);
    }

    /// <summary>
    /// Atualizar pessoa física
    /// </summary>
    [HttpPut("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id,
                                                 [FromBody] IndividualUpdate payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new UpdateIndividualPersonCommand(payload.TenantId,
                                                        id,
                                                        payload.Name ?? string.Empty,
                                                        payload.Email ?? string.Empty,
                                                        payload.PhoneNumber ?? string.Empty,
                                                        payload.MaritalStatus ?? 0);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Excluir pessoa física
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,
                                                 [FromQuery] Guid tenantId,
                                                 CancellationToken cancellationToken)
    {
        var command = new DeleteIndividualPersonCommand(id, tenantId);
        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Buscar pessoa física por identificador
    /// </summary>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IndividualView), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
                                                  [FromQuery] Guid tenantId,
                                                  CancellationToken cancellationToken)
    {
        var query = new GetIndividualByIdQuery(id, tenantId);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(MapIndividual(result.Value));
    }

    /// <summary>
    /// Buscar pessoa física por documento
    /// </summary>
    [HttpGet("document/{document}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IndividualView), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetByDocumentAsync([FromRoute] string document,
                                                        [FromQuery] Guid tenantId,
                                                        CancellationToken cancellationToken)
    {
        var query = new GetIndividualByDocument(document, tenantId);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(MapIndividual(result.Value));
    }

    /// <summary>
    /// Listar pessoas físicas por tenant
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<IndividualView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByTenantAsync([FromQuery] IndividualCollectionFilter filter,
                                                      CancellationToken cancellationToken)
    {
        var query = new GetIndividualByTenantId(filter.TenantId, filter.PageNumber, filter.PageSize);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value.Select(MapIndividual));
    }

    private static IndividualView MapIndividual(IndividualPersonResult result)
        => new(result.id,
               result.tenantId,
               result.name,
               result.document,
               result.type,
               result.email,
               result.phoneNumber,
               result.maritalStatus);
}

