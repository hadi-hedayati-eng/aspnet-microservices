using PlatformService.Domain;

namespace PlatformService.Infrastructure.Data;

public static class DataSeeder
{
    public static void Seed(this IApplicationBuilder applicationBuilder)
    {
        var scope = applicationBuilder.ApplicationServices.CreateScope();

        var _dbContext = scope.ServiceProvider.GetService<PlatformServiceDbContext>();
        if (_dbContext is null)
            return;

        Console.WriteLine("Seeding Data...");

        List<Platform> platforms =
        [
            new("Dotnet", "Microsoft", "Free"),
            new("SQL Server Express", "Microsoft", "Free"),
            new("Kubernetes", "Cloud Native Computing Foundation", "Free")
        ];
        _dbContext.Platforms.AddRange(platforms);

        _dbContext.SaveChanges();

        _dbContext.Dispose();
        scope.Dispose();

        Console.WriteLine("Seeding Data Completed!");
    }
}
