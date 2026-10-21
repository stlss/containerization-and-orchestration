using LabDomain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LabApi.Controllers;

[ApiController]
[Route("")]
public sealed class LabController2(ILabService2 service) : ControllerBase
{
    [HttpGet("fail")]
    public void Fail() => service.Fail();

    [HttpGet("slow")]
    public async Task<IActionResult> Slow(CancellationToken cancellationToken)
    {
        await service.SlowAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("load")]
    public async Task<IActionResult> Load(CancellationToken cancellationToken)
    {
        var requestCounts = await Task.WhenAll(
            service.LoadAsync(GetUri("/health"), 90, cancellationToken),
            service.LoadAsync(GetUri("/slow"), 5, cancellationToken),
            service.LoadAsync(GetUri("/fail"), 5, cancellationToken));

        var requestCount = requestCounts.Sum();
        return Ok(new { requestCount });
    }

    private Uri GetUri(string path) => new(UriHelper.BuildAbsolute(
        Request.Scheme,
        Request.Host,
        Request.PathBase,
        path));
}
