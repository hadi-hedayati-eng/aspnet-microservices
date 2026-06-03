using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Contracts;
using PlatformService.Domain;
using PlatformService.Events;
using PlatformService.Infrastructure.RabbitMQ;
using PlatformService.Infrastructure.Repositories;
using PlatformService.SyncDataServices;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/platforms")]
public class PlatformsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPlatformRepository _platformRepository;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IRabbitMQClient _rabbitMQClient;

    public PlatformsController(
        IMapper mapper,
        IPlatformRepository platformRepository,
        ICommandDataClient commandDataClient,
        IRabbitMQClient rabbitMQClient
    )
    {
        _mapper = mapper;
        _platformRepository = platformRepository;
        _commandDataClient = commandDataClient;
        _rabbitMQClient = rabbitMQClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>?>> GetPlatforms()
    {
        var result = await _platformRepository.GetAll();

        if (!result.Any())
            return Ok(new List<PlatformReadDto>());

        var mappedResult = _mapper.Map<IEnumerable<PlatformReadDto>>(result);
        return Ok(mappedResult);
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public async Task<ActionResult<PlatformReadDto>> GetPlatformById([FromRoute] int id)
    {
        var platform = await _platformRepository.GetById(id);

        if (platform is null)
            return NotFound();

        var mappedPlatform = _mapper.Map<PlatformReadDto>(platform);

        return Ok(mappedPlatform);
    }

    [HttpPost(Name = "CreatePlatform")]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(
        [FromBody] PlatformCreateDto body
    )
    {
        var platform = _mapper.Map<Platform>(body);
        await _platformRepository.CreatePlatform(platform);
        await _platformRepository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

        /* --------------------- Http Synchronous Communication --------------------- */
        await _commandDataClient.SendPlatformToCommand(platformReadDto);

        /* ------------------- RabbitMQ Asynchronous Communication ------------------ */
        var platformCreatedEvent = _mapper.Map<PlatformCreatedEvent>(platformReadDto);
        await _rabbitMQClient.PublishNewPlatform(platformCreatedEvent);

        return CreatedAtRoute(
            routeName: nameof(GetPlatformById),
            new { platformReadDto.Id },
            platformReadDto
        );
    }
}
