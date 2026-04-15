namespace PlatformService.Domain;

public class Platform
{
    public int Id { get; }
    public string Name { get; private set; } = null!;
    public string Publisher { get; private set; } = null!;
    public string Cost { get; private set; } = null!;

    public Platform(string name, string publisher, string cost)
    {
        Name = name;
        Publisher = publisher;
        Cost = cost;
    }

    private Platform() { }
}
