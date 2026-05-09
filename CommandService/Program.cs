using CommandService.Infrastructure;
using CommandService.Infrastructure.RabbitMQ;
using CommandService.Infrastructure.Repositories.Commands;
using CommandService.Infrastructure.Repositories.Platforms;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<CommandServiceDbContext>(
    options => options.UseInMemoryDatabase("CommandInMemory")
);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ICommandRepository, CommandRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

builder.Services.AddHostedService<RabbitMQSubscriber>();

var app = builder.Build();

app.MapControllers();
app.Run();
