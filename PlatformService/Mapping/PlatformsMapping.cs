using AutoMapper;
using PlatformService.Contracts;
using PlatformService.Domain;
using PlatformService.Events;

namespace PlatformService.Mapping;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
        CreateMap<PlatformReadDto, PlatformCreatedEvent>()
            .ForCtorParam("Event", opt => opt.MapFrom(src => "PlatformCreatedEvent"));
    }
}
