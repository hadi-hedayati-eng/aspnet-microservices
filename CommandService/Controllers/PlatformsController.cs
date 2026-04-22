using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("/api/c/platforms")]
public class PlatformsController : ControllerBase
{
    public PlatformsController() { }

    [HttpPost]
    public IActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound Post from PLATFORM to COMMAND Service");

        return StatusCode(201, new { Hello = "World" });
    }
}
