using YukiChan.Shared.Data;
using YukiChan.Shared.Data.Console;

namespace YukiChan.Client.Console.Apis;

public class YukiConsoleRootApi : YukiConsoleBaseApi
{
    internal YukiConsoleRootApi(HttpClient client) : base(client)
    {
    }

    public Task<YukiResponse<PrecheckResponse>> Precheck(PrecheckRequest req) =>
        Post<PrecheckRequest, PrecheckResponse>("v1/precheck", req);
}