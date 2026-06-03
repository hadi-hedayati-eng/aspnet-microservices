using AutoMapper;
using Grpc.Core;
using PlatformService.Infrastructure.Repositories;

namespace PlatformService.Controllers;

public class GrpcPlatformController : GrpcPlatformService.GrpcPlatformServiceBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;

    public GrpcPlatformController(IPlatformRepository platformRepository, IMapper mapper)
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
    }

    public override async Task<GetAllPlatformsResponse> GetAllPlatforms(
        GetAllPlatformsRequest request,
        ServerCallContext context
    )
    {
        var response = new GetAllPlatformsResponse();
        var platforms = await _platformRepository.GetAll();

        foreach (var platform in platforms)
        {
            response.Platforms.Add(_mapper.Map<GrpcPlatformModel>(platform));
        }

        return response;
    }
}
