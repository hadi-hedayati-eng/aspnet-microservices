using Microsoft.EntityFrameworkCore;
using PlatformService.Domain;

namespace PlatformService.Infrastructure.Repositories;

public class PlatformRepository(PlatformServiceDbContext dbContext) : IPlatformRepository
{
    private readonly PlatformServiceDbContext _dbContext = dbContext;

    public async Task CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        await _dbContext.Platforms.AddAsync(platform);
    }

    public async Task<IEnumerable<Platform>> GetAll()
    {
        return await _dbContext.Platforms.ToListAsync();
    }

    public async Task<Platform?> GetById(int id)
    {
        return await _dbContext
            .Platforms.Where(platform => platform.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> SaveChanges()
    {
        var result = await _dbContext.SaveChangesAsync();
        return result >= 0;
    }
}
