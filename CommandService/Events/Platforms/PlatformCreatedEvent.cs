namespace CommandService.Events.Platforms;

public record class PlatformCreatedEvent(int Id, string Name, string Event) : DomainEvent(Event);
