using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules.Malody;

public partial class MalodyModule
{
    [Command("MalodySelfInfo",
        Command = "me",
        Usage = "ma me")]
    public static async Task<MessageBuilder> GetMe(Bot bot, MessageStruct message)
    {
        try
        {
            var user = Global.YukiDb.GetMalodyUser(message.Sender.Uin);
            if (user is null)
                return message.Reply("请先使用 #ma bind <用户名> 绑定您的账号哦~");
            await bot.SendReply(message, $"正在查询 {user.Name} 的用户信息...");
            var userInfo = await MalodyQuery.GetUserInfo(user.Id);

            var mb = message.Reply()
                .Text($"{userInfo.UserName} ({user.Id})\n")
                .Text($"{userInfo.Sex}   {userInfo.Age}岁   居住于{userInfo.LiveIn}\n")
                .Text($"金币: {userInfo.CoinCount}   收入: {userInfo.Income}\n")
                .Text($"加入时间: {userInfo.JoinedTime:yyyy.MM.dd}\n")
                .Text($"最后游玩: {userInfo.LastPlayedTime:yyyy.MM.dd}\n")
                .Text($"总游戏时长: {userInfo.TotalPlayedTime}\n");

            if (userInfo.OnSaleChartsCount + userInfo.NoneOnSaleChartsCount > 0)
                mb.Text($"上架谱面: {userInfo.OnSaleChartsCount} - 非上架谱面: {userInfo.NoneOnSaleChartsCount}\n")
                    .Text($"谱面被游玩总时长: {userInfo.ChartBeingPlayedTime}\n");

            foreach (var mode in userInfo.MalodyUserRanks)
                mb.Text($"\n{mode.Mode} 模式 | #{mode.Rank} | 游玩 {mode.PlayCount} 次\n")
                    .Text($"Exp:{mode.Exp}   Acc:{mode.Acc:N2}%   Combo:{mode.Combo}");

            return mb;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}