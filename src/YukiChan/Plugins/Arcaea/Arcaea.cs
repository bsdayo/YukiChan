using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Client.Console;

namespace YukiChan.Plugins.Arcaea;

public sealed partial class ArcaeaPlugin : Plugin
{
    private readonly ArcaeaService _service;

    private readonly YukiConsoleClient _yukiClient;

    private readonly ILogger<ArcaeaPlugin> _logger;

    public ArcaeaPlugin(ArcaeaService service, YukiConsoleClient yukiClient,
        ILogger<ArcaeaPlugin> logger)
    {
        _service = service;
        _yukiClient = yukiClient;
        _logger = logger;
    }
}