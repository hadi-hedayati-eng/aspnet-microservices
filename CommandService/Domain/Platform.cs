namespace CommandService.Domain;

public class Platform
{
    public int Id { get; }
    public int ExternalId { get; private set; }
    public string Name { get; private set; } = null!;
    public ICollection<Command> Commands { get; private set; } = [];

    public Platform(string name, int externalId)
    {
        Name = name;
        ExternalId = externalId;
    }

    private Platform() { }
}
