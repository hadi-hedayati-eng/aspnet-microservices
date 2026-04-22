using System.Text;
using System.Text.Json;
using PlatformService.Contracts;

namespace PlatformService.SyncDataServices;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platform),
            encoding: Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(
            $"{_configuration["CommandService:Uri"]}/api/c/platforms/",
            httpContent
        );

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("--> Sync POST to CommandService was OK");
            return;
        }

        Console.WriteLine("NOT OKAY!");
    }
}
