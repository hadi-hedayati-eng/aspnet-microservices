using AutoMapper;
using CommandService.Contracts;
using CommandService.Domain;
using CommandService.Events.Platforms;

namespace CommandService.Mapping;

public class PlatformsMapping : Profile
{
    public PlatformsMapping()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreatedEvent, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
    }
}
