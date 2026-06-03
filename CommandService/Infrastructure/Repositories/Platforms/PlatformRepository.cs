using CommandService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Infrastructure.Repositories.Platforms;

public class PlatformRepository : IPlatformRepository
{
    private readonly CommandServiceDbContext _dbContext;

    public PlatformRepository(CommandServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        await _dbContext.Platforms.AddAsync(platform);
    }

    public async Task<IEnumerable<Platform>> GetAllPlatforms()
    {
        return await _dbContext.Platforms.ToListAsync();
    }

    public Task<bool> PlatformExistsByExternalId(int externalId)
    {
        return _dbContext.Platforms.AnyAsync(p => p.Id == externalId);
    }

    public Task<bool> PlatformExistsById(int platformId)
    {
        return _dbContext.Platforms.AnyAsync(p => p.Id == platformId);
    }

    public async Task<bool> SaveChanges()
    {
        return await _dbContext.SaveChangesAsync() >= 0;
    }
}
