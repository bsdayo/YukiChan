﻿using YukiChan.Shared.Data;
using YukiChan.Shared.Data.Console.Guilds;

namespace YukiChan.Client.Console.Apis;

public class YukiConsoleGuildsApi : YukiConsoleBaseApi
{
    internal YukiConsoleGuildsApi(HttpClient client) : base(client)
    {
    }

    public Task<YukiResponse> UpdateAssignee(string platform, string guildId, GuildUpdateAssigneeRequest req) =>
        Put($"v1/guilds/{platform}/{guildId}/authority", req);
}