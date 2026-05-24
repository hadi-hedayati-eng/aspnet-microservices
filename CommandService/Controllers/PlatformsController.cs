using System.Diagnostics.Metrics;
using System.Net;
using AutoMapper;
using CommandService.Contracts;
using CommandService.Infrastructure.Repositories.Platforms;
using CommandService.SyncDataServices;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("/api/c/platforms")]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;
    private readonly IPlatformClient _platformClient;

    private static readonly Meter Meter = new("PlatformController");
    private static readonly Counter<int> PlatformsCreated = Meter.CreateCounter<int>(
        "platforms.created"
    );

    public PlatformsController(
        IMapper mapper,
        IPlatformRepository platformRepository,
        IPlatformClient platformClient
    )
    {
        _mapper = mapper;
        _platformRepository = platformRepository;
        _platformClient = platformClient;
    }

    [HttpPost]
    public IActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound Post from PLATFORM to COMMAND Service");

        foreach (var (key, value) in Request.Headers)
        {
            Console.WriteLine($"{key}: {value}");
        }

        PlatformsCreated.Add(1);

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

    [HttpPost("fetch")]
    public async Task<IActionResult> FetchPlatforms()
    {
        await _platformClient.ReturnAllPlatforms();
        return StatusCode((int)HttpStatusCode.Created);
    }
}
