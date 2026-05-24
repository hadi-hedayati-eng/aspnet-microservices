using CommandService.Domain;

namespace CommandService.SyncDataServices;

public interface IPlatformClient
{
    Task ReturnAllPlatforms();
}
