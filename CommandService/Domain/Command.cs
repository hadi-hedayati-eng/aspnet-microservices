namespace CommandService.Domain;

public class Command
{
    public int Id { get; }
    public string HowTo { get; private set; } = null!;
    public string CommandLine { get; private set; } = null!;
    public int PlatformId { get; set; }
    public Platform? Platform { get; private set; }

    private Command() { }

    public Command(int platformId, string howTo, string commandLine)
    {
        HowTo = howTo;
        PlatformId = platformId;
        CommandLine = commandLine;
    }
}
