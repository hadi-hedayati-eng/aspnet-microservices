using CommandService.Infrastructure;
using CommandService.Infrastructure.RabbitMQ;
using CommandService.Infrastructure.Repositories.Commands;
using CommandService.Infrastructure.Repositories.Platforms;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

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
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = builder.Configuration["Elastic:OpenTelemetry"];
        options.Protocol = OtlpProtocol.Grpc;
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "CommandService"
        };
    })
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.MapControllers();
app.Run();
