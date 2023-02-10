using Spectre.Console;
using Spectre.Console.Cli;
using YukiChan.Client.Console;
using YukiChan.Shared.Data.Console.Arcaea;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Tools.Utils;

namespace YukiChan.Tools.Arcaea;

public sealed class HandleAliasSubmissionCommand : AsyncCommand
{
    private readonly YukiConsoleClient _client;

    public HandleAliasSubmissionCommand(YukiConsoleClient client)
    {
        _client = client;
    }

    public override async Task<int> ExecuteAsync(CommandContext ctx)
    {
        var listResp = await _client.Arcaea.GetAliasSubmissions(
            ArcaeaAliasSubmissionStatus.Pending);

        if (!listResp.Ok)
        {
            LogUtils.Error("获取申请列表时发生了错误。");
            return 1;
        }

        for (var i = 0; i < listResp.Data.Length; i++)
        {
            var submission = listResp.Data[i];
            var remain = listResp.Data.Length - i;

            var songResp = await _client.Arcaea.QuerySong(submission.SongId);
            if (!songResp.Ok)
            {
                LogUtils.Error("获取曲目信息时发生了错误。");
                return 1;
            }

            var title = Markup.Escape(
                $"[blue bold]曲目: [/]{songResp.Data.Difficulties[0].NameEn}   [green bold]别名: [/]{submission.Alias}   [yellow bold]剩余: [/]{remain}");

            var status = AnsiConsole.Prompt(
                new SelectionPrompt<ArcaeaAliasSubmissionStatus>()
                    .Title(title)
                    .AddChoices(
                        ArcaeaAliasSubmissionStatus.Accepted,
                        ArcaeaAliasSubmissionStatus.Rejected,
                        ArcaeaAliasSubmissionStatus.Pending));

            if (status == ArcaeaAliasSubmissionStatus.Pending)
                continue;

            await _client.Arcaea.UpdateAliasSubmission(submission.Id, new ArcaeaUpdateAliasSubmissionRequest
            {
                Status = status
            });
        }

        return 0;
    }
}