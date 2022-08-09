using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using MalodyInfoQuery.Model;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules.Malody;

public partial class MalodyModule
{
    [Command("MalodyRecent",
        Command = "recent",
        Usage = "ma recent",
        FallbackCommand = true)]
    public static async Task<MessageBuilder> GetRecent(Bot bot, MessageStruct message, string body)
    {
        try
        {
            var user = Global.YukiDb.GetMalodyUser(message.Sender.Uin);
            if (user is null)
                return message.Reply("请先使用 #ma bind <用户名> 绑定您的账号哦~");
            await bot.SendReply(message, $"正在查询 {user.Name} 的最近成绩...");
            var recentList = await MalodyQuery.SearchRecent(user.Id);

            if (recentList.Count == 0)
                return message.Reply("您最近还没有打过歌哦~");

            int count;
            if (string.IsNullOrWhiteSpace(body)) count = 1;
            else if (body == "all") count = 100;
            else if (int.TryParse(body, out var c)) count = c;
            else return message.Reply("请输入正确的数量哦~");
            count = count <= 0 ? 1 : count;

            if (count == 1)
            {
                var recent = recentList[0];
                var cover = await MalodyUtils.GetSongCover(recent);
                return message.Reply().Add(ConstructRecent(recent, cover).Build());
            }

            var mlc = new MultiMsgChain();
            for (int i = 0, j = 0; i < recentList.Count && j < count; i++, j++)
            {
                var recent = recentList[i];
                var cover = await MalodyUtils.GetSongCover(recent);
                mlc.AddMessage(bot.Uin, recent.SongName, ConstructRecent(recent, cover).Build());
            }

            return new MessageBuilder(mlc);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }

    private static MessageBuilder ConstructRecent(MalodySongModel recent, byte[] cover)
    {
        return new MessageBuilder()
            .Image(cover)
            .Text($"{recent.SongName}\n")
            .Text($"模式: {recent.Mode}   判定: {recent.Judge}\n")
            .Text($"准确: {recent.Acc:N2}%   连击: {recent.Combo}\n")
            .Text($"得分: {recent.Score}\n")
            .Text($"时间: {recent.PlayedTime} ago");
    }
}