using CommandService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Infrastructure.Repositories.Commands;

public class CommandRepository : ICommandRepository
{
    private readonly CommandServiceDbContext _dbContext;

    public CommandRepository(CommandServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateCommand(int platformId, Command command)
    {
        ArgumentNullException.ThrowIfNull(command);
        command.PlatformId = platformId;
        await _dbContext.Commands.AddAsync(command);
    }

    public async Task<Command?> GetCommand(int platformId, int commandId)
    {
        return await _dbContext
            .Commands.Where(c => c.Id == commandId && c.PlatformId == platformId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Command>> GetCommandsByPlatform(int platformId)
    {
        return await _dbContext.Commands.Where(c => c.PlatformId == platformId).ToListAsync();
    }

    public async Task<bool> SaveChanges()
    {
        return await _dbContext.SaveChangesAsync() >= 0;
    }
}
