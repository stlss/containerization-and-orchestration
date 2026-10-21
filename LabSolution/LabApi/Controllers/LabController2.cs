using LabDomain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LabApi.Controllers;

[ApiController]
[Route("")]
public sealed class LabController2(ILabService2 service) : ControllerBase
{
    [HttpGet("fail")]
    public IActionResult Fail()
    {
        var errorCount = service.RecordError();
        return StatusCode(StatusCodes.Status500InternalServerError, new { errorCount });
    }

    [HttpGet("slow")]
    public async Task<IActionResult> Slow(CancellationToken cancellationToken)
    {
        await service.SlowAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("load")]
    public async Task<IActionResult> Load(CancellationToken cancellationToken)
    {
        var healthUri = GetHealthUri();
        var requestCount = await service.LoadAsync(healthUri, cancellationToken);
        return Ok(new { requestCount });
    }

    private Uri GetHealthUri() => new(UriHelper.BuildAbsolute(
        Request.Scheme,
        Request.Host,
        Request.PathBase,
        "/health"));
}