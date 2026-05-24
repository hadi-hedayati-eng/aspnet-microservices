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

var app = builder.Build();

app.MapControllers();

app.MapGrpcService<GrpcPlatformController>();
app.MapGrpcReflectionService();

app.Seed(builder.Environment.IsProduction());

app.Run();
