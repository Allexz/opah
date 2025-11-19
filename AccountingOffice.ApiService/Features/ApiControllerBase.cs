using System.Net;
using AccountingOffice.Application.Infrastructure.Common;
using AccountingOffice.Application.Infrastructure.ServicesBus.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AccountingOffice.ApiService.Features;

public abstract class ApiControllerBase : ControllerBase
{
    protected ApiControllerBase(IApplicationBus applicationBus)
    {
        ApplicationBus = applicationBus ?? throw new ArgumentNullException(nameof(applicationBus));
    }

    protected IApplicationBus ApplicationBus { get; }

    protected IActionResult FromResult(Result result, HttpStatusCode successStatus = HttpStatusCode.OK)
    {
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return StatusCode((int)successStatus);
    }

    protected IActionResult FromResult<T>(Result<T> result,
                                          Func<T, object>? mapper = null,
                                          HttpStatusCode successStatus = HttpStatusCode.OK)
    {
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        var payload = mapper is null ? result.Value : mapper(result.Value);
        return StatusCode((int)successStatus, payload);
    }
}

