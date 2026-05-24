using AutoMapper;
using CommandService.Contracts;
using CommandService.Domain;
using CommandService.Events.Platforms;
using PlatformService;

namespace CommandService.Mapping;

public class PlatformsMapping : Profile
{
    public PlatformsMapping()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreatedEvent, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
            .ForMember(dest => dest.Commands, opt => opt.Ignore());
    }
}
