using System.Net;
using AccountingOffice.ApiService.Models;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.AccountPay.Commands;
using AccountingOffice.Application.UseCases.AccountPay.Queries;
using AccountingOffice.Application.UseCases.AccountPay.Queries.Result;
using Microsoft.AspNetCore.Mvc;

namespace AccountingOffice.ApiService.Features;

[Route("services/[controller]")]
[ApiController]
public sealed class AccountPayablesController : ApiControllerBase
{
    public AccountPayablesController(IApplicationBus applicationBus) : base(applicationBus)
    {
    }

    /// <summary>
    /// Criar conta a pagar
    /// </summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] AccountPayableCreation payload, CancellationToken cancellationToken)
    {
        var command = new CreateAccountPayableCommand(payload.TenantId,
                                                      payload.SupplierId,
                                                      payload.Description,
                                                      payload.Amount,
                                                      payload.DueDate,
                                                      payload.Status,
                                                      payload.PayMethod,
                                                      payload.PaymentDate);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.Created);
    }

    /// <summary>
    /// Atualizar conta a pagar
    /// </summary>
    [HttpPut("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id,
                                                 [FromBody] AccountPayableUpdate payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new UpdateAccountPayableCommand(payload.TenantId,
                                                      id,
                                                      payload.Description ?? string.Empty,
                                                      payload.Status ?? 0,
                                                      payload.PayMethod ?? 0,
                                                      payload.PaymentDate);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Excluir conta a pagar
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,
                                                 [FromQuery] Guid tenantId,
                                                 CancellationToken cancellationToken)
    {
        var command = new DeleteAccountPayableCommand(id, tenantId);
        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Buscar conta a pagar por identificador
    /// </summary>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AccountPayableView), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,
                                                  [FromQuery] Guid tenantId,
                                                  CancellationToken cancellationToken)
    {
        var query = new GetAccountPayByIdQuery(id, tenantId);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(MapAccount(result.Value));
    }

    /// <summary>
    /// Listar contas a pagar por tenant
    /// </summary>
    [HttpGet("tenant")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<AccountPayableView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByTenantAsync([FromQuery] AccountPayableListFilter filter,
                                                      CancellationToken cancellationToken)
    {
        var query = new GetAccountPayByTenantIdQuery(filter.TenantId, filter.PageNumber, filter.PageSize);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        var payload = result.Value.Select(MapAccount);
        return Ok(payload);
    }

    /// <summary>
    /// Listar contas a pagar por fornecedor
    /// </summary>
    [HttpGet("related-party")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<AccountPayableView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByRelatedPartyAsync([FromQuery] AccountPayableRelatedPartyFilter filter,
                                                            CancellationToken cancellationToken)
    {
        var query = new GetAccountPayByRelatedPartQuery(filter.RelatedPartyId,
                                                        filter.TenantId,
                                                        filter.PageNumber,
                                                        filter.PageSize);

        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        var payload = result.Value.Select(MapAccount);
        return Ok(payload);
    }

    /// <summary>
    /// Listar contas a pagar por data de emissão
    /// </summary>
    [HttpGet("issue-date")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<AccountPayableView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByIssueDateAsync([FromQuery] AccountPayableDateRangeFilter filter,
                                                         CancellationToken cancellationToken)
    {
        var query = new GetAccountPayByIssueDateQuery(filter.StartDate,
                                                      filter.EndDate,
                                                      filter.TenantId,
                                                      filter.PageNumber,
                                                      filter.PageSize);

        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        var payload = result.Value.Select(MapAccount);
        return Ok(payload);
    }

    private static AccountPayableView MapAccount(AccountPayableResult result)
        => new(result.id,
               result.tenantId,
               result.description,
               result.ammount,
               result.issueDate,
               result.dueDate,
               (int)result.status,
               MapPerson(result.supplier),
               (int)result.payMethod,
               result.paymentDate);

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

