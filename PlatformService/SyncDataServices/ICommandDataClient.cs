using PlatformService.Contracts;

namespace PlatformService.SyncDataServices;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}
