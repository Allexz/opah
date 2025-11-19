using System.Net;
using AccountingOffice.ApiService.Models;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Cia.Commands;
using AccountingOffice.Application.UseCases.Cia.Queries;
using AccountingOffice.Application.UseCases.Cia.Queries.Result;
using Microsoft.AspNetCore.Mvc;

namespace AccountingOffice.ApiService.Features;

[Route("services/[controller]")]
[ApiController]
public sealed class CompaniesController : ApiControllerBase
{
    public CompaniesController(IApplicationBus applicationBus) : base(applicationBus)
    {
    }

    /// <summary>
    /// Criar uma nova companhia
    /// </summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCompanyPayload payload, CancellationToken cancellationToken)
    {
        var command = new CreateCompanyCommand(Guid.NewGuid(),
                                               payload.Name,
                                               payload.Document,
                                               payload.Email,
                                               payload.Phone,
                                               payload.Active);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.Created);
    }

    /// <summary>
    /// Atualizar dados de uma companhia
    /// </summary>
    [HttpPut("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id,
                                                 [FromBody] UpdateCompanyPayload payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new UpdateCompanyCommand(id,
                                               payload.Name ?? string.Empty,
                                               payload.Email ?? string.Empty,
                                               payload.Phone ?? string.Empty);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Excluir uma companhia
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCompanyCommand(id);
        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Atualizar status ativo/inativo da companhia
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ToggleStatusAsync([FromRoute] Guid id,
                                                       [FromBody] ToggleCompanyActivePayload payload,
                                                       CancellationToken cancellationToken)
    {
        var command = new ToggleCompanyActiveStatusCommand(id, payload.Active);
        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Buscar companhia pelo identificador
    /// </summary>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CompanyView), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCompanyByIdQuery(id);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(MapCompany(result.Value));
    }

    /// <summary>
    /// Buscar companhia pelo documento
    /// </summary>
    [HttpGet("document/{document}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CompanyView), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetByDocumentAsync([FromRoute] string document, CancellationToken cancellationToken)
    {
        var query = new GetCompanyByDocumentQuery(document);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(MapCompany(result.Value));
    }

    /// <summary>
    /// Listar companhias ativas
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<CompanyView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllAsync([FromQuery] CompanyCollectionFilter filter, CancellationToken cancellationToken)
    {
        var query = new GetAllCompaniesQuery(filter.PageNumber, filter.PageSize);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        var payload = result.Value.Select(MapCompany);
        return Ok(payload);
    }

    private static CompanyView MapCompany(CompanyResult result)
        => new(result.Id,
               result.Name,
               result.Document,
               result.Email,
               result.Phone,
               result.Active,
               result.CreatedAt);
}

