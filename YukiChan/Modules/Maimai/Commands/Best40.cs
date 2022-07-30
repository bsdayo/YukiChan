using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Modules.Maimai.Models;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Maimai;

public partial class MaimaiModule
{
    [Command("Best40",
        Command = "b40",
        Description = "查询 Best40 成绩",
        Usage = "mai b40 [用户名]",
        Example = "mai b40")]
    public static async Task<MessageBuilder> Best40(Bot bot, MessageStruct message, string body)
    {
        string username;

        if (string.IsNullOrWhiteSpace(body))
        {
            var user = Global.YukiDb.GetMaimaiUser(message.Sender.Uin);
            if (user is null)
                return message.Reply("请先使用 #mai bind <用户名> 绑定您的账号哦~\n")
                    .Text("您也可以使用 #mai b40 <用户名> 直接进行查询。");
            username = user.Name;
        }
        else
        {
            username = body;
        }

        try
        {
            var best40 = await DivingFishClient.B40(username);

            var rootMultiMsgChain = MultiMsgChain.Create();
            var dxMultiMsgChain = MultiMsgChain.Create();
            var sdMultiMsgChain = MultiMsgChain.Create();

            var infoMsg = new MessageBuilder()
                .Text($"{best40.Username}\n")
                .Text($"底分: {best40.Rating}\n")
                .Text($"段位分: {best40.AdditionalRating}\n")
                .Build();

            rootMultiMsgChain.AddMessage(new MessageStruct(bot.Uin, "基本信息", infoMsg));

            var getChartMb = async (MaimaiChart chart) =>
            {
                var mb = new MessageBuilder();
                var cover = await File.ReadAllBytesAsync(
                    $"Assets/Maimai/SongCover/{MaimaiUtils.GetChart4DigitId(chart.SongId)}.png");

                var honors = "";
                if (!string.IsNullOrWhiteSpace(chart.FullCombo))
                    honors += MaimaiUtils.GetHonorText(chart.FullCombo) + " ";
                if (!string.IsNullOrWhiteSpace(chart.FullSync))
                    honors += MaimaiUtils.GetHonorText(chart.FullSync);
                honors = honors.Trim().Replace(" ", "/");
                if (!string.IsNullOrWhiteSpace(honors))
                    honors = $"[{honors}]";

                return mb.Image(cover)
                    .Text($"{chart.Title}\n")
                    .Text($"{chart.LevelLabel} {chart.Level} [{chart.Const}]\n")
                    .Text($"{chart.Achievements}% -> {MaimaiUtils.GetGradeText(chart.Grade)} {honors}\n")
                    .Text($"Rating: {chart.Rating}\n")
                    .Text($"DX分: {chart.DxScore}");
            };

            foreach (var chart in best40.Charts.Dx)
            {
                var mb = await getChartMb(chart);
                dxMultiMsgChain.AddMessage(new MessageStruct(bot.Uin, chart.Title, mb.Build()));
            }

            foreach (var chart in best40.Charts.Standard)
            {
                var mb = await getChartMb(chart);
                sdMultiMsgChain.AddMessage(new MessageStruct(bot.Uin, chart.Title, mb.Build()));
            }

            rootMultiMsgChain.Add(((bot.Uin, bot.Name), dxMultiMsgChain));
            rootMultiMsgChain.Add(((bot.Uin, bot.Name), sdMultiMsgChain));

            return new MessageBuilder(rootMultiMsgChain);
        }
        catch (DivingFishApiException e)
        {
            return message.Reply("获取用户 Best40 时发生了错误：" + e.Message);
        }
        catch (YukiException e)
        {
            Logger.Error(e);
            return message.Reply(e.Message);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}