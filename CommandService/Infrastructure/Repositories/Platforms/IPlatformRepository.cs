using CommandService.Domain;

namespace CommandService.Infrastructure.Repositories.Platforms;

public interface IPlatformRepository
{
    bool SaveChanges();
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExistsById(int platformId);
    bool PlatformExistsByExternalId(int externalId);
}
