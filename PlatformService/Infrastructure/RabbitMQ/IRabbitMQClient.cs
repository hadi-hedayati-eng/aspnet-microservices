using PlatformService.Events;

namespace PlatformService.Infrastructure.RabbitMQ;

public interface IRabbitMQClient
{
    void PublishNewPlatform(PlatformCreatedEvent @event);
}
