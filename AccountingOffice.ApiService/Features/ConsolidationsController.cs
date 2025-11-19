using System.Net;
using AccountingOffice.ApiService.Models;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using AccountingOffice.Application.UseCases.Consolidation.Queries;
using Microsoft.AspNetCore.Mvc;

namespace AccountingOffice.ApiService.Features;

[Route("services/[controller]")]
[ApiController]
public sealed class ConsolidationsController : ApiControllerBase
{
    public ConsolidationsController(IApplicationBus applicationBus) : base(applicationBus)
    {
    }

    /// <summary>
    /// Buscar movimento diário consolidado por TenantId
    /// </summary>
    [HttpGet("daily/{tenantId:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(DailyConsolidationView), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetDailyConsolidationAsync(
        [FromRoute] Guid tenantId,
        [FromQuery] DateTime? date = null,
        CancellationToken cancellationToken = default)
    {
        // Se não informar a data, usa a data atual
        var targetDate = date ?? DateTime.UtcNow.Date;

        var query = new GetDailyConsolidationQuery(tenantId, targetDate);
        var result = await ApplicationBus.SendQuery(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        if (result.Value is null)
            return NotFound(new { error = "Não foi encontrado movimento consolidado para a data informada." });

        return Ok(MapToView(result.Value));
    }

    private static DailyConsolidationView MapToView(Application.UseCases.Consolidation.Queries.Result.DailyConsolidationResult result)
        => new(result.TenantId,
               result.Date,
               result.TotalPayable,
               result.TotalReceivable,
               result.Balance,
               result.PayableCount,
               result.ReceivableCount);
}

