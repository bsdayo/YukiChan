using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using YukiChan.Plugins.Arcaea.Models.Database;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.set <preferences:string>")]
    public async Task<MessageContent> OnSet(MessageContext ctx, ParsedArgs args)
    {
        var prefStrList = args.GetArgument<string>("preferences")
            .Replace('，', ',')
            .Split(',');

        var pref = await Global.YukiDb.GetArcaeaUserPreferences(
            ctx.Platform, ctx.UserId) ?? new ArcaeaUserPreferences();

        bool ParseBoolSet(string[] set)
            => set.Length <= 1 || !set[1].Equals("false", StringComparison.OrdinalIgnoreCase);

        foreach (var prefStr in prefStrList)
        {
            var set = prefStr.Split('=');
            switch (set[0].ToLower())
            {
                case "dark":
                    pref.Dark = ParseBoolSet(set);
                    break;

                case "nya":
                    pref.Nya = ParseBoolSet(set);
                    break;
            }
        }

        await Global.YukiDb.AddOrUpdateArcaeaUserPreferences(ctx.Platform, ctx.UserId, pref);
        return ctx.Reply("已成功为您更新偏好信息。");
    }
}