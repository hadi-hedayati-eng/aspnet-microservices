using CommandService.Domain;

namespace CommandService.Infrastructure.Repositories.Platforms;

public class PlatformRepository : IPlatformRepository
{
    private readonly CommandServiceDbContext _dbContext;

    public PlatformRepository(CommandServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        _dbContext.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _dbContext.Platforms.ToList();
    }

    public bool PlatformExistsByExternalId(int externalId)
    {
        return _dbContext.Platforms.Any(p => p.Id == externalId);
    }

    public bool PlatformExistsById(int platformId)
    {
        return _dbContext.Platforms.Any(p => p.Id == platformId);
    }

    public bool SaveChanges()
    {
        return _dbContext.SaveChanges() >= 0;
    }
}
