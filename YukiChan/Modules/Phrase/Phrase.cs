using System.Text;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules.Phrase;

[Module("Phrase",
    Command = "phrase",
    Description = "短语生成",
    Version = "1.3.0")]
public class Phrase : ModuleBase
{
    private static readonly ModuleLogger Logger = new("Phrase");
    
    [Command("Generate Phrase",
        Description = "生成短语",
        FallbackCommand = true)]
    public static MessageBuilder Generate(Bot bot, MessageStruct message, string body)
    {
        try
        {
            var sb = new StringBuilder();

            sb.Append(Global.YukiDb.GetPhraseWord("time"));

            sb.Append(Global.YukiDb.GetPhraseWord("adj"));
            sb.Append(Global.YukiDb.GetPhraseWord("noun"));

            sb.Append(Global.YukiDb.GetPhraseWord("place"));

            sb.Append(new Random().Next() % 2 == 0 ? '把' : '被');

            sb.Append(Global.YukiDb.GetPhraseWord("adj"));
            sb.Append(Global.YukiDb.GetPhraseWord("noun"));

            sb.Append(Global.YukiDb.GetPhraseWord("adv"));
            sb.Append(Global.YukiDb.GetPhraseWord("verb"));

            return message.Reply(sb.ToString());

        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生错误！({e.GetType().Name}: {e.Message})");
        }
    }

    [Command("Add Phrase Word",
        Command = "add")]
    public static MessageBuilder AddWord(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);

        if (args.Length < 2)
            return message.Reply("参数缺失！");

        var type = args[0] switch
        {
            "n" => "noun",
            "v" => "verb",
            _ => args[0]
        };

        if (!new[] { "noun", "verb", "adj", "adv", "time", "place" }.Contains(type))
            return message.Reply("输入了错误的词性哦！");

        Global.YukiDb.AddPhraseWord(args[1], type);

        return message.Reply("已添加词汇：" + args[1]);
    }

    [Command("Remove Phrase Word",
        Command = "remove")]
    public static MessageBuilder RemoveWord(Bot bot, MessageStruct message, string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return message.Reply("请输入需要移除的词汇哦~");

        try
        {
            Global.YukiDb.RemovePhraseWord(body);
        }
        catch
        {
            return message.Reply("移除失败！");
        }

        return message.Reply("移除成功！");
    }
}