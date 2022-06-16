using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;

namespace YukiChan.Modules.Yuki;

public partial class YukiModule
{
    [Command("Status",
        Command = "status",
        Description = "查看暮雪酱状态")]
    public static MessageBuilder ShowStatus(Bot bot, MessageStruct message)
    {
        return StatusModule.ShowStatus(bot, message);
    }
}