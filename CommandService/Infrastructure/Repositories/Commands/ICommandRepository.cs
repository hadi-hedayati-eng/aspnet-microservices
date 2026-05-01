using CommandService.Domain;

namespace CommandService.Infrastructure.Repositories.Commands;

public interface ICommandRepository
{
    bool SaveChanges();
    IEnumerable<Command> GetCommandsByPlatform(int platformId);
    Command? GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}
