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

    private readonly IPlatformRepository _platformRepository;

    public CommandsController(
        IMapper mapper,
        ICommandRepository commandRepository,
        IPlatformRepository platformRepository
    )
    {
        _mapper = mapper;
        _commandRepository = commandRepository;
        _platformRepository = platformRepository;
    }

    [HttpGet("/{platformId}/commands")]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommands([FromRoute] int platformId)
    {
        var platformExists = _platformRepository.PlatformExists(platformId);

        if (!platformExists)
        {
            return NotFound();
        }

        var commands = _commandRepository.GetCommandsByPlatform(platformId);

        var commandsReadObjects = _mapper.Map<IEnumerable<CommandReadDto>>(commands);

        return Ok(commandsReadObjects);
    }

    [HttpGet("/{platformId}/commands/{commandId}")]
    public ActionResult<CommandReadDto> GetCommand(
        [FromRoute] int platformId,
        [FromRoute] int commandId
    )
    {
        var platformExists = _platformRepository.PlatformExists(platformId);
        if (!platformExists)
        {
            return NotFound();
        }

        var command = _commandRepository.GetCommand(platformId, commandId);
        if (command is null)
        {
            return NotFound();
        }

        var commandsReadObjects = _mapper.Map<CommandReadDto>(command);

        return Ok(commandsReadObjects);
    }

    [HttpPost("{platformId}/commands")]
    public ActionResult<CommandReadDto> CreateCommand(
        [FromRoute] int platformId,
        [FromBody] CommandCreateDto commandCreateDto
    )
    {
        var platformExists = _platformRepository.PlatformExists(platformId);
        if (!platformExists)
            return NotFound();

        var command = _mapper.Map<Command>(commandCreateDto);

        _commandRepository.CreateCommand(platformId, command);
        _commandRepository.SaveChanges();

        var commandReadObject = _mapper.Map<CommandReadDto>(command);

        return CreatedAtAction(
            nameof(GetCommand),
            new { platformId, commandId = commandReadObject.Id },
            commandReadObject
        );
    }
}
