using PlatformService.Domain;

namespace PlatformService.Infrastructure.Repositories;

public interface IPlatformRepository
{
    Task<bool> SaveChanges();
    Task<IEnumerable<Platform>> GetAll();
    Task<Platform?> GetById(int id);
    Task CreatePlatform(Platform platform);
}
