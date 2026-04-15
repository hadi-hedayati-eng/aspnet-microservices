using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Contracts;
using PlatformService.Domain;
using PlatformService.Infrastructure.Repositories;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/platforms")]
public class PlatformsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPlatformRepository _platformRepository;

    public PlatformsController(IMapper mapper, IPlatformRepository platformRepository)
    {
        _mapper = mapper;
        _platformRepository = platformRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>?> GetPlatforms()
    {
        var result = _platformRepository.GetAll();

        if (!result.Any())
            return Ok(new List<PlatformReadDto>());

        var mappedResult = _mapper.Map<IEnumerable<PlatformReadDto>>(result);
        return Ok(mappedResult);
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById([FromRoute] int id)
    {
        var platform = _platformRepository.GetById(id);

        if (platform is null)
            return NotFound();

        var mappedPlatform = _mapper.Map<PlatformReadDto>(platform);

        return Ok(mappedPlatform);
    }

    [HttpPost(Name = "CreatePlatform")]
    public IActionResult CreatePlatform([FromBody] PlatformCreateDto body)
    {
        var platform = _mapper.Map<Platform>(body);
        _platformRepository.CreatePlatform(platform);
        _platformRepository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

        return CreatedAtRoute(
            routeName: nameof(GetPlatformById),
            new { platformReadDto.Id },
            platformReadDto
        );
    }
}
