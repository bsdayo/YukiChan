using System.Linq;
using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Attributes;
using YukiChan.Core;
using YukiChan.Models;

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
        var helpCommands = body
            .Split(' ')
            .Where(item => !string.IsNullOrEmpty(item))
            .ToArray();

        switch (helpCommands.Length)
        {
            case 0:
                return ModuleManager.GetHelp();

            case 1:
                return ModuleManager.Modules
                           .FirstOrDefault(module => module.ModuleInfo.Command == helpCommands[0])
                           ?.GetHelp()
                       ?? new MessageBuilder()
                           .Add(ReplyChain.Create(message))
                           .Text("未找到指定的模块，请检查模块名称。");

            case 2:
                return ModuleManager.Modules
                           .FirstOrDefault(module => module.ModuleInfo.Command == helpCommands[0])
                           ?.Commands
                           .FirstOrDefault(command => command.CommandInfo.Command == helpCommands[1])
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
}