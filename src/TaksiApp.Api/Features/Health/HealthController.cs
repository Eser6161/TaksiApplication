using Microsoft.AspNetCore.Mvc;

namespace TaksiApp.Api.Features.Health;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<object> GetHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Service = "TaksiApp API"
        });
    }

    [HttpGet("detailed")]
    public ActionResult<object> GetDetailedHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Service = "TaksiApp API",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64),
            MachineName = Environment.MachineName
        });
    }
}