using CommandService.Infrastructure;
using CommandService.Infrastructure.RabbitMQ;
using CommandService.Infrastructure.Repositories.Commands;
using CommandService.Infrastructure.Repositories.Platforms;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<CommandServiceDbContext>(
    options => options.UseInMemoryDatabase("CommandInMemory")
);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ICommandRepository, CommandRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

builder.Services.AddHostedService<RabbitMQSubscriber>();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(
        new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
        {
            IndexFormat = "command-service-logs-{0:yyyy-MM}",
            AutoRegisterTemplate = true,
            NumberOfReplicas = 0,
            NumberOfShards = 1
        }
    )
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.MapControllers();
app.Run();
