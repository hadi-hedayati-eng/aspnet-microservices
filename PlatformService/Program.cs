using Microsoft.EntityFrameworkCore;
using PlatformService.Infrastructure;
using PlatformService.Infrastructure.Data;
using PlatformService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PlatformServiceDbContext>(
    opt => opt.UseInMemoryDatabase("PlatformInMemoryDB")
);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Seed();

app.Run();
