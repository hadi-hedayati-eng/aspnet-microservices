using AutoMapper;
using PlatformService.Contracts;
using PlatformService.Domain;

namespace PlatformService.Mapping;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
    }
}
