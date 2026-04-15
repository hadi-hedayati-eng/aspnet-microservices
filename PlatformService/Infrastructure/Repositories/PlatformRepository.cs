using PlatformService.Domain;

namespace PlatformService.Infrastructure.Repositories;

public class PlatformRepository(PlatformServiceDbContext dbContext) : IPlatformRepository
{
    private readonly PlatformServiceDbContext _dbContext = dbContext;

    public void CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        _dbContext.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAll()
    {
        return _dbContext.Platforms.ToList();
    }

    public Platform? GetById(int id)
    {
        return _dbContext.Platforms.Where(platform => platform.Id == id).FirstOrDefault();
    }

    public bool SaveChanges()
    {
        var result = _dbContext.SaveChanges();
        return result >= 0;
    }
}
