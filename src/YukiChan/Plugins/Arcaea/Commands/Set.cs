using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using YukiChan.Shared.Data.Console.Arcaea;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.set")]
    public async Task<MessageContent> OnSet(MessageContext ctx, string preferences)
    {
        var prefStrList = preferences
            .Replace('，', ',')
            .Replace('；', ',')
            .Replace(';', ',')
            .Split(',');

        var prefResp = await _yukiClient.Arcaea.GetPreferences(ctx.Platform, ctx.UserId);
        if (!prefResp.Ok) return ctx.ReplyServerError(prefResp);

        var pref = prefResp.Data.Preferences;

        bool ParseBoolSet(string[] set)
            => set.Length <= 1 || !set[1].Equals("false", StringComparison.OrdinalIgnoreCase);

        foreach (var prefStr in prefStrList)
        {
            var set = prefStr.Split('=');
            switch (set[0].ToLower().Replace("_", ""))
            {
                case "dark":
                    pref.Dark = ParseBoolSet(set);
                    break;

                case "nya":
                    pref.Nya = ParseBoolSet(set);
                    break;

                case "singledynamicbg":
                    pref.SingleDynamicBackground = ParseBoolSet(set);
                    break;

                case "b30showgrade":
                    pref.Best30ShowGrade = ParseBoolSet(set);
                    break;
            }
        }

        var updateResp = await _yukiClient.Arcaea.UpdatePreferences(ctx.Platform, ctx.UserId,
            new ArcaeaPreferencesRequest { Preferences = pref });
        if (!updateResp.Ok) ctx.ReplyServerError(updateResp);
        return ctx.Reply("已成功为您更新偏好信息。");
    }
}