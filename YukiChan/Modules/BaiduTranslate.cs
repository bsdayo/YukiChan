using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

namespace YukiChan.Modules;

[Module("BaiduTranslate",
    Command = "trans",
    Description = "文本翻译",
    Version = "1.0.0")]
public class BaiduTranslateModule : ModuleBase
{
    private static readonly string[][] LanguageMap =
    {
        // langCode, langName, ...langAlias
        new[] { "auto", "自动", "自动检测" },
        new[] { "zh", "中文", "简中" },
        new[] { "en", "英语", "英文" },
        new[] { "yue", "粤语" },
        new[] { "wyw", "文言文" },
        new[] { "jp", "日语", "日文" },
        new[] { "kor", "韩语", "韩文" },
        new[] { "fra", "法语", "法文" },
        new[] { "spa", "西班牙语" },
        new[] { "th", "泰语" },
        new[] { "ara", "阿拉伯语" },
        new[] { "ru", "俄语", "俄文" },
        new[] { "pt", "葡萄牙语" },
        new[] { "de", "德语", "德文" },
        new[] { "it", "意大利语" },
        new[] { "el", "希腊语" },
        new[] { "nl", "荷兰语" },
        new[] { "pl", "波兰语" },
        new[] { "bul", "保加利亚语" },
        new[] { "est", "爱沙尼亚语" },
        new[] { "dan", "丹麦语" },
        new[] { "fin", "芬兰语" },
        new[] { "cs", "捷克语" },
        new[] { "rom", "罗马尼亚语" },
        new[] { "slo", "斯洛文尼亚语" },
        new[] { "swe", "瑞典语" },
        new[] { "hu", "匈牙利语" },
        new[] { "cht", "繁体中文", "繁中" },
        new[] { "vie", "越南语" }
    };

    private static readonly ModuleLogger Logger = new("BaiduTranslate");

    private static string GetLangCode(string lang)
    {
        return LanguageMap.FirstOrDefault(l => l.Contains(lang))?[0] ?? lang;
    }

    private static string GetLangName(string lang)
    {
        return LanguageMap.FirstOrDefault(l => l.Contains(lang))?[1] ?? lang;
    }

    [Command("BaiduTranslate",
        Description = "文本翻译",
        StartsWith = "翻译",
        Usage = "trans [目标语言] <文本>")]
    public static async Task<MessageBuilder> BaiduTranslate(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);

        string sourceLang, targetLang, text;

        switch (args.Length)
        {
            case 0:
                return message.Reply("请输入需要翻译的目标语言和文本哦~");

            case 1:
                sourceLang = "auto";
                targetLang = "zh";
                text = args[0];
                break;

            default:
                if (args[0].Contains('：'))
                {
                    var langs = args[0].Split('：');
                    sourceLang = GetLangCode(langs[0]);
                    targetLang = GetLangCode(langs[1]);
                }
                else if (args[0].Contains(':'))
                {
                    var langs = args[0].Split(':');
                    sourceLang = GetLangCode(langs[0]);
                    targetLang = GetLangCode(langs[1]);
                }
                else
                {
                    sourceLang = "auto";
                    targetLang = GetLangCode(args[0]);
                }

                if (string.IsNullOrWhiteSpace(sourceLang))
                    sourceLang = "auto";
                if (string.IsNullOrWhiteSpace(targetLang))
                    targetLang = "auto";

                text = args[1];
                break;
        }

        if (string.IsNullOrWhiteSpace(sourceLang))
            return message.Reply("源语言输入错误，请检查重试。");
        if (string.IsNullOrWhiteSpace(targetLang))
            return message.Reply("目标语言输入错误，请检查重试。");

        try
        {
            var salt = new Random()
                .NextInt64(10000000000)
                .ToString()
                .PadLeft(10, '0');
            var sign = BitConverter.ToString(
                    MD5.Create().ComputeHash(Encoding.Default.GetBytes(
                        Global.YukiConfig.BaiduTranslate.AppId +
                        text + salt +
                        Global.YukiConfig.BaiduTranslate.Token)))
                .Replace("-", "")
                .ToLower();

            Logger.Debug($"Source Language: {sourceLang}");
            Logger.Debug($"Target Language: {targetLang}");
            Logger.Debug($"Text to translate: {text}");
            Logger.Debug($"APP ID: {Global.YukiConfig.BaiduTranslate.AppId}");
            Logger.Debug($"Token: {Global.YukiConfig.BaiduTranslate.Token}");
            Logger.Debug($"Salt: {salt}");
            Logger.Debug($"Sign: {sign}");

            var client = new HttpClient();
            var query = new QueryBuilder()
                .Add("q", text)
                .Add("from", sourceLang)
                .Add("to", targetLang)
                .Add("appid", Global.YukiConfig.BaiduTranslate.AppId)
                .Add("salt", salt)
                .Add("sign", sign)
                .Build();

            var resp = await client.GetStringAsync(
                "https://fanyi-api.baidu.com/api/trans/vip/translate" + query);

            var data = JsonSerializer.Deserialize<BaiduTranslateResponse>(resp)!;

            if (data.ErrorCode is not null && data.ErrorCode != "52000")
            {
                var errorMessage = data.ErrorCode switch
                {
                    "52001" => "API 请求超时。",
                    "52002" => "系统错误。",
                    "52003" => "用户未授权，请联系开发者。",
                    "54000" => "参数错误，请联系开发者。",
                    "54001" => "签名错误，请联系开发者。",
                    "54003" => "API 调用频率达到限制，请稍后再试。",
                    "54004" => "API 账户余额不足，请联系开发者。",
                    "54005" => "短时间内翻译长文本的频率过高，请稍后再试。",
                    "58000" => "请求源 IP 未在白名单内，请联系开发者。",
                    "58001" => "不支持的翻译语言方向。",
                    "58002" => "服务已关闭，请联系开发者。",
                    "90107" => "API 认证失败，请联系开发者。",
                    _ => "未知错误。"
                };
                Logger.Error(errorMessage);
                return message.Reply(errorMessage);
            }

            return message.Reply()
                .Text($"翻译结果 [{GetLangName(data.From)} > {GetLangName(data.To)}]\n")
                .Text(data.Result[0].Dst);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}

#pragma warning disable CS8618
// ReSharper disable once UnusedAutoPropertyAccessor.Global
internal class BaiduTranslateResponse
{
    [JsonPropertyName("from")] public string From { get; set; }

    [JsonPropertyName("to")] public string To { get; set; }

    [JsonPropertyName("trans_result")] public BaiduTranslateResult[] Result { get; set; }

    [JsonPropertyName("error_code")] public string? ErrorCode { get; set; }

    [JsonPropertyName("error_msg")] public string? ErrorMessage { get; set; }

    internal class BaiduTranslateResult
    {
        [JsonPropertyName("src")] public string Src { get; set; }

        [JsonPropertyName("dst")] public string Dst { get; set; }
    }
}