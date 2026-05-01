using CommandService.Domain;

namespace CommandService.Infrastructure.Repositories.Commands;

public class CommandRepository : ICommandRepository
{
    private readonly CommandServiceDbContext _dbContext;

    public CommandRepository(CommandServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void CreateCommand(int platformId, Command command)
    {
        ArgumentNullException.ThrowIfNull(command);
        command.PlatformId = platformId;
        _dbContext.Commands.Add(command);
    }

    public Command? GetCommand(int platformId, int commandId)
    {
        return _dbContext
            .Commands.Where(c => c.Id == commandId && c.PlatformId == platformId)
            .FirstOrDefault();
    }

    public IEnumerable<Command> GetCommandsByPlatform(int platformId)
    {
        return _dbContext.Commands.Where(c => c.PlatformId == platformId);
    }

    public bool SaveChanges()
    {
        return _dbContext.SaveChanges() >= 0;
    }
}
