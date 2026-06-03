using CommandService.Domain;

namespace CommandService.Infrastructure.Repositories.Commands;

public interface ICommandRepository
{
    Task<bool> SaveChanges();
    Task<IEnumerable<Command>> GetCommandsByPlatform(int platformId);
    Task<Command?> GetCommand(int platformId, int commandId);
    Task CreateCommand(int platformId, Command command);
}
