using Microsoft.EntityFrameworkCore;
using PlatformService.Infrastructure;
using PlatformService.Infrastructure.Data;
using PlatformService.Infrastructure.RabbitMQ;
using PlatformService.Infrastructure.Repositories;
using PlatformService.SyncDataServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PlatformServiceDbContext>(opt =>
{
    if (builder.Environment.IsDevelopment())
    {
        Console.WriteLine(builder.Configuration["Database:ConnectionString"]);
        // opt.UseSqlServer(connectionString: builder.Configuration["Database:ConnectionString"]);
        opt.UseInMemoryDatabase("InMemoryDatabase");
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

var app = builder.Build();

app.MapControllers();

app.Seed(builder.Environment.IsProduction());

app.Run();
