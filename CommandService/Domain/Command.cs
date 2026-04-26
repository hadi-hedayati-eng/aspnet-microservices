namespace CommandService.Domain;

public class Command
{
    public int Id { get; }
    public string HowTo { get; private set; }
    public string CommandLine { get; private set; }
    public int PlatformId { get; private set; }
    public Platform Platform { get; private set; }
}
