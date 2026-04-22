using Microsoft.EntityFrameworkCore;
using PlatformService.Infrastructure;
using PlatformService.Infrastructure.Data;
using PlatformService.Infrastructure.Repositories;
using PlatformService.SyncDataServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PlatformServiceDbContext>(
    opt => opt.UseInMemoryDatabase("PlatformInMemoryDB")
);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

builder.Services.AddControllers();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService:Uri"]}");

var app = builder.Build();

app.MapControllers();

app.Seed();

app.Run();
