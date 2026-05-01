using AutoMapper;
using CommandService.Contracts;
using CommandService.Domain;

namespace CommandService.Mapping;

public class PlatformsMapping : Profile
{
    public PlatformsMapping()
    {
        CreateMap<Platform, PlatformReadDto>();
    }
}
