using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Database;

namespace YukiChan.Plugins.Arcaea;

public sealed partial class ArcaeaPlugin : Plugin
{
    private readonly ArcaeaService _service;

    private readonly YukiDbManager _database;

    private readonly ILogger<ArcaeaPlugin> _logger;

    public ArcaeaPlugin(ArcaeaService service, YukiDbManager database, ILogger<ArcaeaPlugin> logger)
    {
        _service = service;
        _database = database;
        _logger = logger;
    }
}