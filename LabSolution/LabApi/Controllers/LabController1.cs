using System.Net.Mime;
using LabDomain.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabApi.Controllers;

[ApiController]
[Route("")]
public sealed class LabController1(ILabService1 service) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        var healthStatus = service.GetHealthStatus();
        return Content($"{healthStatus}{Environment.NewLine}", MediaTypeNames.Text.Plain);
    }

    [HttpGet("eat")]
    public IActionResult Eat([FromQuery] int mb = 0)
    {
        if (mb < 0) return BadRequest("mb must be non-negative.");
        service.EatMemory(mb);
        return NoContent();
    }

    [HttpGet("burn")]
    public IActionResult Burn()
    {
        service.BurnCpu();
        return NoContent();
    }
}
