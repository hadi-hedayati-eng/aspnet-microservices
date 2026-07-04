using System.Diagnostics;
using System.Threading.RateLimiting;
using CommandService.Infrastructure;
using CommandService.Infrastructure.RabbitMQ;
using CommandService.Infrastructure.Repositories.Commands;
using CommandService.Infrastructure.Repositories.Platforms;
using CommandService.Logging;
using CommandService.SyncDataServices;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
builder.Services.AddScoped<IPlatformClient, GrpcPlatformClient>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    .WithMetrics(
        m =>
            m.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddOtlpExporter(
                    o => o.Endpoint = new Uri(builder.Configuration["Elastic:OpenTelemetry"])
                )
    )
    .WithTracing(
        t =>
            t.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRabbitMQInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
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

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter(
        "fixed",
        opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 10;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 0;
        }
    );

    options.AddSlidingWindowLimiter(
        "sliding",
        opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 10;
            opt.SegmentsPerWindow = 6; // 6 x 10s Segments
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 0;
        }
    );

    options.AddTokenBucketLimiter(
        "token",
        opt =>
        {
            opt.TokenLimit = 10; // max token
            opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
            opt.TokensPerPeriod = 2; // refill rate
            opt.AutoReplenishment = true;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 0;
        }
    );

    options.AddConcurrencyLimiter(
        "concurrency",
        opt =>
        {
            opt.PermitLimit = 5;
            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            opt.QueueLimit = 0;
        }
    );
});

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

app.UseRateLimiter();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
