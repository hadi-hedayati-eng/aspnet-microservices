using PlatformService.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.Infrastructure.RabbitMQ;

public class RabbitMQClient : IRabbitMQClient, IHostedService, IDisposable
{
    private readonly IConfiguration _configuration;
    private IConnection _connection;

    public RabbitMQClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public void PublishNewPlatform(PlatformCreatedEvent @event)
    {
        throw new NotImplementedException();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"]!,
            Port = int.Parse(_configuration["RabbitMQ:Port"]!)
        };
        _connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        using var channel = await CreateChannel();

        await channel.ExchangeDeclareAsync(
            exchange: "trigger",
            type: ExchangeType.Fanout,
            cancellationToken: cancellationToken
        );

        _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutDown;

        Console.WriteLine("Connected To Message Bus");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Closing RabbitMQ");
        await _connection.CloseAsync(cancellationToken);
    }

    public async Task<IChannel> CreateChannel()
    {
        var channel = await _connection.CreateChannelAsync();
        return channel;
    }

    private Task RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs @event)
    {
        Console.WriteLine("RabbitMQ Connection Shutdown");
        return Task.CompletedTask;
    }
}
