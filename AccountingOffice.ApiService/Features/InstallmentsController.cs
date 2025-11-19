using System.Net;
using AccountingOffice.ApiService.Models;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Installm.Commands;
using AccountingOffice.Application.UseCases.Installments.Commands;
using AccountingOffice.Application.UseCases.Installm.Queries;
using AccountingOffice.Application.UseCases.Installm.Queries.Result;
using Microsoft.AspNetCore.Mvc;

namespace AccountingOffice.ApiService.Features;

[Route("services/[controller]")]
[ApiController]
public sealed class InstallmentsController : ApiControllerBase
{
    public InstallmentsController(IApplicationBus applicationBus) : base(applicationBus)
    {
    }

    /// <summary>
    /// Adicionar parcela a uma conta
    /// </summary>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] InstallmentCreation payload,
                                              CancellationToken cancellationToken)
    {
        var command = new AddInstallmentCommand(payload.AccountId,
                                                payload.TenantId,
                                                payload.EntryType,
                                                payload.InstallmentNumber,
                                                payload.Amount,
                                                payload.DueDate);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.Created);
    }

    /// <summary>
    /// Atualizar status da parcela
    /// </summary>
    [HttpPatch("status")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ChangeStatusAsync([FromBody] InstallmentStatusChange payload,
                                                       CancellationToken cancellationToken)
    {
        var command = new ChangeInstallmentStatusCommand(payload.AccountId,
                                                         payload.TenantId,
                                                         payload.InstallmentNumber,
                                                         payload.Status);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Registrar pagamento de parcela
    /// </summary>
    [HttpPost("payments")]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PayAsync([FromBody] InstallmentPayment payload,
                                              CancellationToken cancellationToken)
    {
        var command = new PayInstallmentCommand(payload.AccountId,
                                                payload.TenantId,
                                                payload.InstallmentNumber,
                                                payload.PaymentDate,
                                                payload.PaymentAmount);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Remover parcela
    /// </summary>
    [HttpDelete]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteAsync([FromBody] InstallmentDeletion payload,
                                                 CancellationToken cancellationToken)
    {
        var command = new DeleteInstallmentCommand(payload.AccountId,
                                                   payload.TenantId,
                                                   payload.InstallmentNumber,
                                                   payload.PaymentDate);

        var result = await ApplicationBus.SendCommand(command, cancellationToken);
        return FromResult(result, successStatus: HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Listar parcelas por conta
    /// </summary>
    [HttpGet("account")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<InstallmentView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByAccountAsync([FromQuery] InstallmentAccountFilter filter,
                                                       CancellationToken cancellationToken)
    {
        var query = new GetByAccountIdQuery(filter.TenantId,
                                            filter.AccountId,
                                            filter.PageNumber,
                                            filter.PageSize);

        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value.Select(MapInstallment));
    }

    /// <summary>
    /// Listar parcelas por tenant
    /// </summary>
    [HttpGet("tenant")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<InstallmentView>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByTenantAsync([FromQuery] InstallmentTenantFilter filter,
                                                      CancellationToken cancellationToken)
    {
        var query = new GetInstallmentsByTenantIdQuery(filter.TenantId,
                                                       filter.PageNumber,
                                                       filter.PageSize);

        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value.Select(MapInstallment));
    }

    private static InstallmentView MapInstallment(InstallmentResult result)
        => new(result.InstallmentNumber,
               result.Amount,
               result.DueDate,
               result.Status,
               result.PaymentDate,
               (int)result.Entrytype);
}

