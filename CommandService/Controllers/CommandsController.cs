using System.Diagnostics;
using AutoMapper;
using CommandService.Contracts;
using CommandService.Domain;
using CommandService.Infrastructure.Repositories.Commands;
using CommandService.Infrastructure.Repositories.Platforms;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("/api/c/platforms")]
public class CommandsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICommandRepository _commandRepository;
    private static readonly ActivitySource _activitySource = new ActivitySource(
        "CommandService.CommandsController"
    );
    private readonly IPlatformRepository _platformRepository;
    private readonly ILogger<CommandsController> _logger;

    public CommandsController(
        IMapper mapper,
        ICommandRepository commandRepository,
        IPlatformRepository platformRepository,
        ILogger<CommandsController> logger
    )
    {
        _mapper = mapper;
        _commandRepository = commandRepository;
        _platformRepository = platformRepository;
        _logger = logger;
    }

    [HttpGet("{platformId}/commands")]
    public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommands(
        [FromRoute] int platformId
    )
    {
        var platformExists = await _platformRepository.PlatformExistsById(platformId);

        if (!platformExists)
        {
            return NotFound();
        }

        var commands = await _commandRepository.GetCommandsByPlatform(platformId);

        var commandsReadObjects = _mapper.Map<IEnumerable<CommandReadDto>>(commands);

        return Ok(commandsReadObjects);
    }

    [HttpGet("{platformId}/commands/{commandId}")]
    public async Task<ActionResult<CommandReadDto>> GetCommand(
        [FromRoute] int platformId,
        [FromRoute] int commandId
    )
    {
        var platformExists = await _platformRepository.PlatformExistsById(platformId);
        if (!platformExists)
        {
            return NotFound();
        }

        var command = await _commandRepository.GetCommand(platformId, commandId);
        if (command is null)
        {
            return NotFound();
        }

        var commandsReadObjects = _mapper.Map<CommandReadDto>(command);

        return Ok(commandsReadObjects);
    }

    [HttpPost("{platformId}/commands")]
    public async Task<ActionResult<CommandReadDto>> CreateCommand(
        [FromRoute] int platformId,
        [FromBody] CommandCreateDto commandCreateDto
    )
    {
        using var activity = _activitySource.StartActivity("Create Command", ActivityKind.Consumer);
        var platformExists = await _platformRepository.PlatformExistsById(platformId);
        if (!platformExists)
            return NotFound();

        _logger.LogInformation("Creating Command In the Commands Controller!!!");

        var command = _mapper.Map<Command>(commandCreateDto);

        await _commandRepository.CreateCommand(platformId, command);
        await _commandRepository.SaveChanges();

        var commandReadObject = _mapper.Map<CommandReadDto>(command);

        return CreatedAtAction(
            nameof(GetCommand),
            new { platformId, commandId = commandReadObject.Id },
            commandReadObject
        );
    }
}
