using Microsoft.Extensions.Options;
using YukiChan.Client.Console.Apis;

namespace YukiChan.Client.Console;

public sealed class YukiConsoleClient : IDisposable
{
    private readonly HttpClient _client;

    public YukiConsoleRootApi Root { get; }
    public YukiConsoleAssetsApi Assets { get; }
    public YukiConsoleArcaeaApi Arcaea { get; }
    public YukiConsoleGuildsApi Guilds { get; }

    public YukiConsoleClient(IOptions<YukiConsoleClientOptions> options)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(new Uri(options.Value.ApiUrl), new Uri("console/", UriKind.Relative));
        _client.Timeout = TimeSpan.FromSeconds(options.Value.Timeout);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.Value.Token}");

        Root = new YukiConsoleRootApi(_client);
        Assets = new YukiConsoleAssetsApi(_client);
        Arcaea = new YukiConsoleArcaeaApi(_client);
        Guilds = new YukiConsoleGuildsApi(_client);
    }

    public void Dispose() => _client.Dispose();
}