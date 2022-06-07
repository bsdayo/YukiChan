using System;
using System.Text;
using System.Threading.Tasks;
using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Attributes;
using YukiChan.Models;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("GitHub",
    Command = "github",
    Description = "GitHub 相关功能",
    Version = "1.0.0")]
public class GitHubModule : ModuleBase
{
    [Command("GitHub",
        Contains = "https://github.com/",
        Description = "获取 GitHub OpenGraph 图片",
        Usage = "github <url>",
        Example = "github https://github.com/b1acksoil/YukiChan")]
    public static async Task<MessageBuilder> GetGitHubImage(Bot bot, MessageStruct message, string body)
    {
        if (!body.StartsWith("https://github.com/"))
            return MessageBuilder.Eval("Not a github link.");

        try
        {
            var htmlBytes = await NetUtils.Download(body);
            var html = Encoding.UTF8.GetString(htmlBytes);

            var metaData = html.GetMetaData("property");
            var imageMeta = metaData["og:image"];

            var image = await NetUtils.Download(imageMeta);

            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Image(image);
        }
        catch (Exception exception)
        {
            BotLogger.Error(exception);
            return MessageBuilder.Eval("Error occurred: " + exception.Message);
        }
    }
}