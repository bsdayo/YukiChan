using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("Help",
    Command = "help",
    Description = "显示帮助")]
public class HelpModule : ModuleBase
{
    [Command("Help",
        Usage = "help [module] [command]",
        Description = "显示帮助")]
    public static MessageBuilder? Help(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);

        switch (args.Length)
        {
            case 0:
                return ModuleManager.GetHelp();

            case 1:
                if (args[0] == "all")
                    return GetAllHelps(bot);
                return ModuleManager.Modules
                           .FirstOrDefault(module => module.ModuleInfo.Command == args[0])
                           ?.GetHelp()
                       ?? new MessageBuilder()
                           .Add(ReplyChain.Create(message))
                           .Text("未找到指定的模块，请检查模块名称。");

            case 2:
                return ModuleManager.Modules
                           .FirstOrDefault(module => module.ModuleInfo.Command == args[0])
                           ?.Commands
                           .FirstOrDefault(command => command.CommandInfo.Command == args[1])
                           ?.GetHelp()
                       ?? new MessageBuilder()
                           .Add(ReplyChain.Create(message))
                           .Text("未找到指定的模块，请检查模块名称。");

            default:
                return new MessageBuilder()
                    .Add(ReplyChain.Create(message))
                    .Text("Invalid parameter length.");
        }
    }

    private static MessageBuilder GetAllHelps(Bot bot)
    {
        var multiMsgChain = MultiMsgChain.Create();
        foreach (var module in ModuleManager.Modules)
            multiMsgChain.Add(((bot.Uin, module.ModuleInfo.Name), module.GetHelp(false).Build()));

        return new MessageBuilder(multiMsgChain);
    }
}