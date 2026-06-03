using AutoMapper;
using CommandService.Domain;
using CommandService.Infrastructure.Repositories.Platforms;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices;

public class GrpcPlatformClient : IPlatformClient
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public GrpcPlatformClient(
        IPlatformRepository platformRepository,
        IMapper mapper,
        IConfiguration configuration
    )
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task ReturnAllPlatforms()
    {
        var channel = GrpcChannel.ForAddress(_configuration["PlatformService:GrpcEndpoint"]!);
        var client = new GrpcPlatformService.GrpcPlatformServiceClient(channel);
        var request = new GetAllPlatformsRequest();
        request.Hello = "Hello Mother Father";

        var response = await client.GetAllPlatformsAsync(request);

        var platforms = _mapper.Map<IEnumerable<Platform>>(response.Platforms);
        foreach (var platform in platforms)
        {
            if (await _platformRepository.PlatformExistsByExternalId(platform.ExternalId))
                continue;
            await _platformRepository.CreatePlatform(platform);
        }
        await _platformRepository.SaveChanges();
    }
}
