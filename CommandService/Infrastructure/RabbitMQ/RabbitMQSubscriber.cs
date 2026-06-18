using System.Text;
using System.Text.Json;
using AutoMapper;
using CommandService.Domain;
using CommandService.Events;
using CommandService.Events.Platforms;
using CommandService.Infrastructure.Repositories.Platforms;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.Infrastructure.RabbitMQ;

public class RabbitMQSubscriber : IRabbitMQSubscriber, IHostedService, IAsyncDisposable
{
    private IConnection _connection = null!;
    private IChannel _channel = null!;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;

    public RabbitMQSubscriber(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        IMapper mapper
    )
    {
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection.IsOpen)
        {
            await _connection.DisposeAsync();
        }

        if (_channel.IsOpen)
        {
            await _channel.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var queueName = "commands-queue";
        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"]!,
            Port = int.Parse(_configuration["RabbitMQ:Port"]!)
        };

        _connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(
            "trigger",
            type: ExchangeType.Fanout,
            cancellationToken: cancellationToken
        );
        await _channel.QueueDeclareAsync(
            queueName,
            cancellationToken: cancellationToken,
            exclusive: false,
            durable: true
        );

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceived;

        await _channel.QueueBindAsync(
            queueName,
            "trigger",
            "",
            cancellationToken: cancellationToken
        );

        await _channel.BasicConsumeAsync(queueName, false, consumer, cancellationToken);
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs @event)
    {
        var body = Encoding.UTF8.GetString(@event.Body.ToArray());
        var domainEvent = JsonSerializer.Deserialize<DomainEvent>(body)!;

        /* --------------------------------- Level 1 -------------------------------- */
        //     IDomainEvent? evt = message.Type switch
        // {
        //     "OrderCreated" => JsonSerializer.Deserialize<OrderCreated>(message.Payload),
        //     "OrderShipped" => JsonSerializer.Deserialize<OrderShipped>(message.Payload),
        //     _ => null
        // };

        /* --------------------------------- Level 2 -------------------------------- */
        // var typeMap = new Dictionary<string, Type>
        // {
        //     ["OrderCreated"] = typeof(OrderCreated),
        //     ["OrderShipped"] = typeof(OrderShipped),
        // };

        /* --------------------------------- Level 3 -------------------------------- */
        // var typeMap = typeof(RabbitMQSubscriber)
        //     .Assembly.GetTypes()
        //     .Where(t => t.IsAssignableTo(typeof(IDomainEvent)) && !t.IsAbstract)
        //     .ToDictionary(t => t.Name, t => t);


        if (domainEvent.Event == EventsEnum.PlatformCreatedEvent.ToString())
        {
            var platformCreatedEvent = JsonSerializer.Deserialize<PlatformCreatedEvent>(body)!;
            using var scope = _serviceScopeFactory.CreateScope();
            var platformRepository =
                scope.ServiceProvider.GetRequiredService<IPlatformRepository>();

            var platform = _mapper.Map<Platform>(platformCreatedEvent);
            var doesPlatformExists = await platformRepository.PlatformExistsByExternalId(
                platform.ExternalId
            );
            if (!doesPlatformExists)
            {
                await platformRepository.CreatePlatform(platform);
                await platformRepository.SaveChanges();
            }
        }

        await _channel.BasicAckAsync(@event.DeliveryTag, multiple: false);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channel.CloseAsync(cancellationToken: cancellationToken);
        await _connection.CloseAsync(cancellationToken: cancellationToken);
    }
}
