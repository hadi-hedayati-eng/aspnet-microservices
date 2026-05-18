using System.Diagnostics;
using CommandService.Infrastructure;
using CommandService.Infrastructure.RabbitMQ;
using CommandService.Infrastructure.Repositories.Commands;
using CommandService.Infrastructure.Repositories.Platforms;
using CommandService.Logging;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Configuration;
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
    .Enrich.With<ActivityEnricher>()
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

builder
    .Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("CommandService"))
    .WithTracing(
        t =>
            t.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(
                    o => o.Endpoint = new Uri(builder.Configuration["Elastic:OpenTelemetry"])
                )
    );

// In Program.cs
ActivitySource.AddActivityListener(
    new ActivityListener
    {
        ShouldListenTo = _ => true,
        Sample = (ref ActivityCreationOptions<ActivityContext> _) =>
            ActivitySamplingResult.AllDataAndRecorded,
    }
);

// // Option A: quick debug
// ActivitySource.AddActivityListener(new ActivityListener
// {
//     ShouldListenTo = _ => true,
//     Sample = (ref ActivityCreationOptions<ActivityContext> _) =>
//         ActivitySamplingResult.AllDataAndRecorded
// });

// // Option B: proper OTel
// builder.Services.AddOpenTelemetry()
//     .WithTracing(b => b.AddSource("CommandService.RabbitMQ"));

var app = builder.Build();

app.MapControllers();
app.Run();
