using ArcaeaUnlimitedAPI.Lib;
using Microsoft.Extensions.Options;
using YukiChan.ImageGen.Arcaea;
using YukiChan.Shared.Arcaea;

namespace YukiChan.Plugins.Arcaea;

public sealed class ArcaeaService : IDisposable
{
    public AuaClient AuaClient { get; private set; } = null!;

    public AlaClient AlaClient { get; private set; } = null!;

    public ArcaeaImageGenerator ImageGenerator { get; }

    public ArcaeaReportManager ReportManager { get; }

    private readonly IDisposable? _optionsMonitor;

    public ArcaeaService(IOptionsMonitor<ArcaeaPluginOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor.OnChange(UpdateClients);
        UpdateClients(optionsMonitor.CurrentValue, null);

        ImageGenerator = new ArcaeaImageGenerator();
        ReportManager = new ArcaeaReportManager();
    }

    private void UpdateClients(ArcaeaPluginOptions options, string? _)
    {
        AuaClient = new AuaClient
        {
            ApiUrl = options.AuaApiUrl,
            UserAgent = options.AuaToken,
            Token = options.AuaToken,
            Timeout = options.AuaTimeout
        }.Initialize();

        AlaClient = new AlaClient
        {
            Token = options.AlaToken,
            Timeout = options.AlaTimeout
        }.Initialize();
    }

    public void Dispose()
    {
        _optionsMonitor?.Dispose();
    }
}