namespace CommandService.Domain;

public class Platform
{
    public int Id { get; }
    public int ExternalId { get; }
    public string Name { get; private set; }
    public ICollection<Command> Commands { get; set; } = [];

    public Platform(string name, int externalId)
    {
        ExternalId = externalId;
        Name = name;
    }

    private Platform() { }
}
