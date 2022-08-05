// ReSharper disable CheckNamespace

using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Modules.Arcaea;

public partial class ArcaeaModule
{
    [Command("AddAlias",
        Command = "addalias",
        Description = "增加曲目别名",
        Usage = "a addalias <曲名> <别名>",
        Authority = YukiUserAuthority.Admin,
        Example = "a addalias lostdesire 失欲")]
    public static MessageBuilder AddAlias(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);
        try
        {
            if (args.Length == 0)
                return message.Reply("请输入需要添加的曲目和别名哦~");
            if (args.Length == 1)
                return message.Reply("请输入需要添加的别名哦~");

            var songId = ArcaeaSongDatabase.FuzzySearchId(string.Join(" ", args[..^1]));
            var alias = args[^1];

            if (songId is null)
                return message.Reply("没有找到该曲目呢...");

            Logger.Info($"为曲目 {songId} 添加别名 {alias}");
            ArcaeaSongDatabase.AddAlias(songId, alias);

            return message.Reply($"已为曲目 {songId} 添加别名 {alias}。");
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}