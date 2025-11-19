using System.Net;
using AccountingOffice.ApiService.Models;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.AccountReceiv.Commands;
using AccountingOffice.Application.UseCases.AccountReceiv.Queries;
using AccountingOffice.Application.UseCases.AccountReceiv.Queries.Result;
using Microsoft.AspNetCore.Mvc;

namespace AccountingOffice.ApiService.Features;

[Route("services/[controller]")]
[ApiController]
public sealed class AccountReceivablesController : ApiControllerBase
{
    public AccountReceivablesController(IApplicationBus applicationBus) : base(applicationBus)
    {
    }

    /// <summary>
    /// Criar conta a receber
    /// </summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] AccountReceivableCreation payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new CreateAccountReceivableCommand(payload.TenantId,
                                                         payload.Description,
                                                         payload.Amount,
                                                         payload.DueDate,
                                                         payload.IssueDate,
                                                         payload.CustomerId,
                                                         payload.PayMethod,
                                                         payload.InvoiceNumber);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.Created);
    }

    /// <summary>
    /// Atualizar conta a receber
    /// </summary>
    [HttpPut("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id,
                                                 [FromBody] AccountReceivableUpdate payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new UpdateAccountReceivableCommand(id,
                                                         payload.TenantId,
                                                         payload.Description ?? string.Empty,
                                                         payload.DueDate ?? default,
                                                         payload.PayMethod ?? 0);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Excluir conta a receber
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,
                                                 [FromQuery] Guid tenantId,
                                                 CancellationToken cancellationToken)
    {
        var command = new DeleteAccountReceivableCommand(id, tenantId);
        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Buscar conta a receber por identificador
    /// </summary>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AccountReceivableView), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
                                                  [FromQuery] Guid tenantId,
                                                  CancellationToken cancellationToken)
    {
        var query = new GetAccountReceivByIdQuery(id, tenantId);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(MapReceivable(result.Value));
    }

    /// <summary>
    /// Listar contas a receber por tenant
    /// </summary>
    [HttpGet("tenant")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<AccountReceivableView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByTenantAsync([FromQuery] AccountReceivableListFilter filter,
                                                      CancellationToken cancellationToken)
    {
        var query = new GetAccountReceivByTenantId(filter.TenantId, filter.PageNumber, filter.PageSize);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value.Select(MapReceivable));
    }

    /// <summary>
    /// Listar contas a receber por cliente
    /// </summary>
    [HttpGet("related-party")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<AccountReceivableView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByRelatedPartyAsync([FromQuery] AccountReceivableRelatedPartyFilter filter,
                                                            CancellationToken cancellationToken)
    {
        var query = new GetAccountReceivByRelatedPartyQuery(filter.RelatedPartyId,
                                                            filter.TenantId,
                                                            filter.PageNumber,
                                                            filter.PageSize);

        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value.Select(MapReceivable));
    }

    /// <summary>
    /// Listar contas a receber por período de emissão
    /// </summary>
    [HttpGet("issue-date")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<AccountReceivableView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByIssueDateAsync([FromQuery] AccountReceivableDateRangeFilter filter,
                                                         CancellationToken cancellationToken)
    {
        var query = new GetAccountReceivByIssueDateQuery(filter.StartDate,
                                                         filter.EndDate,
                                                         filter.TenantId,
                                                         filter.PageNumber,
                                                         filter.PageSize);

        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value.Select(MapReceivable));
    }

    private static AccountReceivableView MapReceivable(AccountReceivableResult result)
        => new(result.id,
               result.tenantId,
               result.description,
               result.ammount,
               result.issueDate,
               result.dueDate,
               result.status,
               MapPerson(result.customer),
               result.payMethod,
               result.invoiceNumber,
               result.receivedDate);

    private static PersonView MapPerson(AccountingOffice.Domain.Core.Aggregates.Person<Guid> person)
        => new(person.Id,
               person.TenantId,
               person.Name,
               person.Document,
               (int)person.Type,
               person.Email,
               person.Phone,
               person.Active);
}

