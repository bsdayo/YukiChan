using ArcaeaUnlimitedAPI.Lib.Models;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using YukiChan.Plugins.Arcaea.Images;
using YukiChan.Plugins.Arcaea.Models;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("b30 <usercode: string>")]
    [Option("nya", "-n <:bool>")]
    public async Task<MessageContent> OnBest30(MessageContext ctx, ParsedArgs args)
    {
        var userId = args.GetArgument<string>("usercode");
        var nya = args.GetOption<bool>("nya");

        var best30 = await _auaClient.User.Best30(userId, 9, AuaReplyWith.All);
        var image = await ArcaeaImageGenerator.Best30(
            ArcaeaBest30.FromAua(best30), _auaClient, false, nya, Logger);

        return new MessageBuilder().Image(ImageSegment.FromData(image));
    }
}