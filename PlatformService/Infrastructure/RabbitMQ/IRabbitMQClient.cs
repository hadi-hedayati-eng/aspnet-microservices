using PlatformService.Events;

namespace PlatformService.Infrastructure.RabbitMQ;

public interface IRabbitMQClient
{
    Task PublishNewPlatform(PlatformCreatedEvent @event);
}
