using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YukiChan.Shared.Utils;

namespace YukiChan.Plugins;

public class GosenPlugin : Plugin
{
    private static readonly HttpClient Client = new();

    private readonly GosenPluginOptions _options;

    private readonly ILogger<GosenPlugin> _logger;

    public GosenPlugin(IOptionsSnapshot<GosenPluginOptions> options, ILogger<GosenPlugin> logger) =>
        (_options, _logger) = (options.Value, logger);

    [Command("5k <upper:string> <lower:string>")]
    [Alias("5K")]
    [Alias("gosen")]
    [Shortcut("五千兆")]
    [Shortcut("五千兆元")]
    [Option("offset", "-o <offset:int>")]
    public async Task<MessageContent> On5K(CommandContext ctx, ParsedArgs args)
    {
        try
        {
            var upper = args.GetArgument<string>("upper");
            var lower = args.GetArgument<string>("lower");
            var offset = args.GetOption<int>("offset");

            var image = await Client.GetByteArrayAsync(
                $"{_options.ApiUrl}/?upper={upper}&lower={lower}&offset={offset}");

            return ctx.Reply().Image(image);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "获取五千兆图片时发生错误");
            return $"获取五千兆图片时发生错误...\n({e.GetType().Name}: {e.Message})";
        }
    }
}

public class GosenPluginOptions
{
    public string ApiUrl { get; set; } = string.Empty;
}