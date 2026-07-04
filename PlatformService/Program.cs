using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PlatformService.Controllers;
using PlatformService.Infrastructure;
using PlatformService.Infrastructure.Data;
using PlatformService.Infrastructure.RabbitMQ;
using PlatformService.Infrastructure.Repositories;
using PlatformService.Logging;
using PlatformService.SyncDataServices;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PlatformServiceDbContext>(opt =>
{
    if (builder.Environment.IsDevelopment())
    {
        Console.WriteLine(builder.Configuration["Database:ConnectionString"]);
        // opt.UseSqlServer(connectionString: builder.Configuration["Database:ConnectionString"]);
        opt.UseSqlServer(connectionString: builder.Configuration["Database:ConnectionString"]);
        // opt.UseInMemoryDatabase("InMemoryDatabase");
    }
    else if (builder.Environment.IsProduction())
    {
        Console.WriteLine(builder.Configuration["Database:ConnectionString"]);
        opt.UseSqlServer(connectionString: builder.Configuration["Database:ConnectionString"]);
    }
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

builder.Services.AddControllers();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddSingleton<RabbitMQClient>();
builder.Services.AddSingleton<IRabbitMQClient>(p => p.GetRequiredService<RabbitMQClient>());
builder.Services.AddHostedService(p => p.GetRequiredService<RabbitMQClient>());

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService:Uri"]}");

Log.Logger = new LoggerConfiguration()
    .Enrich.With<ActivityEnricher>()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = builder.Configuration["Elastic:OpenTelemetry"];
        options.Protocol = OtlpProtocol.Grpc;
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "PlatformService"
        };
    })
    .CreateLogger();

builder.Host.UseSerilog();

builder
    .Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("PlatformService"))
    .WithMetrics(
        m =>
            m.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddOtlpExporter(
                    o => o.Endpoint = new Uri(builder.Configuration["Elastic:OpenTelemetry"]!)
                )
    )
    .WithTracing(
        t =>
            t.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRabbitMQInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(
                    o => o.Endpoint = new Uri(builder.Configuration["Elastic:OpenTelemetry"]!)
                )
    );

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRateLimiter();
app.MapControllers()
    .RequireRateLimiting("token")
    .RequireRateLimiting("fixed")
    .RequireRateLimiting("sliding")
    .RequireRateLimiting("concurrency");

app.MapGrpcService<GrpcPlatformController>();
app.MapGrpcReflectionService();

app.Seed(builder.Environment.IsProduction());

app.Run();
