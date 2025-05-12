using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("job")]
public class JobController : ControllerBase
{
    private static object? latestJobSpec = null;

    [HttpPost]
    public IActionResult Post([FromBody] object jobSpec)
    {
        var now = DateTime.UtcNow;
        var jobSpecWithTimestamp = new
        {
            jobSpec,
            submitted_at = now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        };
        latestJobSpec = jobSpecWithTimestamp;
        return Ok(new { status = "Job Spec received" });
    }

    public static object? GetLatestJobSpec() => latestJobSpec;
}
