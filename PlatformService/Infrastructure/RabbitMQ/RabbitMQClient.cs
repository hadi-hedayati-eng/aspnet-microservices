using System.Text;
using System.Text.Json;
using PlatformService.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.Infrastructure.RabbitMQ;

public sealed class RabbitMQClient : IRabbitMQClient, IHostedService, IDisposable
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
        Console.WriteLine("Disposing");
    }

    public async Task PublishNewPlatform(PlatformCreatedEvent @event)
    {
        using var channel = await CreateChannel();
        var eventString = JsonSerializer.Serialize(@event);
        var eventBytes = Encoding.UTF8.GetBytes(eventString);
        await channel.BasicPublishAsync(exchange: "trigger", routingKey: "", body: eventBytes);
        Console.WriteLine("Sending Message");
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
        if (_connection.IsOpen)
        {
            await _connection.CloseAsync(cancellationToken);
        }
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
