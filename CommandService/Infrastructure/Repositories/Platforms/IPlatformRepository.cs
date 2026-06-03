using CommandService.Domain;

namespace CommandService.Infrastructure.Repositories.Platforms;

public interface IPlatformRepository
{
    Task<bool> SaveChanges();
    Task<IEnumerable<Platform>> GetAllPlatforms();
    Task CreatePlatform(Platform platform);
    Task<bool> PlatformExistsById(int platformId);
    Task<bool> PlatformExistsByExternalId(int externalId);
}
