using AutoMapper;
using CommandService.Contracts;
using CommandService.Infrastructure.Repositories.Platforms;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("/api/c/platforms")]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;

    public PlatformsController(IMapper mapper, IPlatformRepository platformRepository)
    {
        _mapper = mapper;
        _platformRepository = platformRepository;
    }

    [HttpPost]
    public IActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound Post from PLATFORM to COMMAND Service");

        foreach (var (key, value) in Request.Headers)
        {
            Console.WriteLine($"{key}: {value}");
        }

        return StatusCode(201, new { Hello = "World" });
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("-- Getting Platforms from CommandService");

        var platforms = _platformRepository.GetAllPlatforms();

        var platformReadObjects = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);

        return Ok(platformReadObjects);
    }
}
