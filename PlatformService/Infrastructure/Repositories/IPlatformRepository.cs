using PlatformService.Domain;

namespace PlatformService.Infrastructure.Repositories;

public interface IPlatformRepository
{
    bool SaveChanges();

    IEnumerable<Platform> GetAll();
    Platform? GetById(int id);
    void CreatePlatform(Platform platform);
}
