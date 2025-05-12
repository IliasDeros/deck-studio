using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("latest")]
public class LatestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var latestJobSpec = JobController.GetLatestJobSpec();
        if (latestJobSpec is null)
            return NotFound();
        return Ok(latestJobSpec);
    }
}
