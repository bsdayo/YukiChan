using System.Text;
using System.Threading.Tasks;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Attributes;
using YukiChan.Models;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("GitHub",
    Command = "github",
    Description = "Get GitHub repository image",
    Version = "1.0.0")]
public class GitHubModule : ModuleBase
{
    [Command("GitHub",
        Contains = "https://github.com/",
        Description = "Get GitHub repository image",
        Example = "Send https://github.com/b1acksoil/YukiChan")]
    public static async Task<MessageBuilder> GetGitHubImage(Bot bot, MessageStruct message, string body)
    {
        if (!body.StartsWith("https://github.com/"))
            return MessageBuilder.Eval("Not a github repository link.");
        
        try
        {
            var htmlBytes = await NetUtils.Download(body);
            var html = Encoding.UTF8.GetString(htmlBytes);

            var metaData = html.GetMetaData("property");
            var imageMeta = metaData["og:image"];

            var image = await NetUtils.Download(imageMeta);
        
            return new MessageBuilder().Image(image);
        }
        catch
        {
            return MessageBuilder.Eval("Not a github repository link.");
        }
    }
}