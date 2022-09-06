using System.Text;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("Phrase",
    Command = "phrase",
    Description = "短语生成",
    Version = "1.3.0")]
public class Phrase : ModuleBase
{
    private static readonly List<string> Noun = new();
    private static readonly List<string> Adj = new();
    private static readonly List<string> Adv = new();
    private static readonly List<string> Verb = new();
    private static readonly List<string> Place = new();
    private static readonly List<string> Time = new();

    [Command("Generate Phrase",
        Description = "生成短语",
        FallbackCommand = true)]
    public static MessageBuilder Generate(Bot bot, MessageStruct message, string body)
    {
        if (!string.IsNullOrWhiteSpace(body))
        {
            var args = CommonUtils.ParseCommandBody(body);
            switch (args[0])
            {
                case "add":
                    switch (args.Length)
                    {
                        case <= 2:
                            return message.Reply("参数缺失！");

                        default:
                            var list = GetWordList(args[1]);
                            if (list is null)
                                return message.Reply("输入了错误的词性哦！");
                            if (args[2].HasSensitiveWords())
                                return message.Reply("输入了敏感词汇哦！");

                            list.Add(args[2]);

                            return message.Reply("已添加词汇：" + args[2]);
                    }
                case "clear":
                case "reset":
                    Noun.Clear();
                    Adj.Clear();
                    Adv.Clear();
                    Verb.Clear();
                    Time.Clear();
                    return message.Reply("已成功清除短语列表。");

                default:
                    return message.Reply("未知操作！");
            }
        }

        var sb = new StringBuilder();
        var rd = new Random();

        if (Time.Count > 0)
            sb.Append(Time[rd.Next(Time.Count)]);

        if (Adj.Count > 0)
            sb.Append(Adj[rd.Next(Adj.Count)]);
        if (Noun.Count > 0)
            sb.Append(Noun[rd.Next(Noun.Count)]);

        if (Place.Count > 0)
            sb.Append(Place[rd.Next(Place.Count)]);

        sb.Append(rd.Next() % 2 == 0 ? '把' : '被');

        if (Adj.Count > 0)
            sb.Append(Adj[rd.Next(Adj.Count)]);
        if (Noun.Count > 0)
            sb.Append(Noun[rd.Next(Noun.Count)]);

        if (Adv.Count > 0)
            sb.Append(Adv[rd.Next(Adv.Count)]);
        if (Verb.Count > 0)
            sb.Append(Verb[rd.Next(Verb.Count)]);

        return message.Reply(sb.ToString());
    }

    private static List<string>? GetWordList(string keyword)
    {
        return keyword.ToLower() switch
        {
            "n" => Noun,
            "noun" => Noun,
            "adj" => Adj,
            "adv" => Adv,
            "v" => Verb,
            "verb" => Verb,
            "time" => Time,
            "place" => Place,
            _ => null
        };
    }
}