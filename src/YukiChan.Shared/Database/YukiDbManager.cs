﻿using System.Reflection;
using Chloe.Annotations;
using Chloe.SQLite;
using Chloe.SQLite.DDL;
using Flandre.Core.Messaging;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Database.Models;

namespace YukiChan.Shared.Database;

[YukiDatabase(GuildDataDbName, typeof(GuildData))]
[YukiDatabase(UserDataDbName, typeof(UserData))]
[YukiDatabase(CommandHistoryDbName, typeof(CommandHistory))]
public partial class YukiDbManager
{
    private readonly ILogger<YukiDbManager> _logger;

    private const string CommandHistoryDbName = "command-history";
    private const string GuildDataDbName = "guilds";
    private const string UserDataDbName = "users";

    public YukiDbManager(ILogger<YukiDbManager> logger)
    {
        _logger = logger;

        var attrs = GetType().GetCustomAttributes<YukiDatabaseAttribute>(false);

        foreach (var attr in attrs)
        {
            using var ctx = GetDbContext(attr.Name);
            _logger.LogDebug("更新数据库 {DbPath}/{DbName}.db",
                YukiDir.Databases, attr.Name);

            foreach (var tableType in attr.TableTypes)
            {
                var tableAttr = tableType.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttr is not null ? tableAttr.Name : tableType.Name;
                new SQLiteTableGenerator(ctx).CreateTable(tableType, tableName);
                _logger.LogDebug("  通过类 {ClassName} 创建表 {TableName}",
                    tableType.Name, tableName);
            }
        }
    }

    private static SQLiteContext GetDbContext(string dbName)
    {
        return new SQLiteContext(() => new SqliteConnection($"DataSource={YukiDir.Databases}/{dbName}.db"));
    }

    public async Task<GuildData?> GetGuildData(string platform, string guildId)
    {
        using var ctx = GetDbContext(GuildDataDbName);
        return await ctx.Query<GuildData>()
            .Where(guild => guild.Platform == platform)
            .Where(guild => guild.GuildId == guildId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<GuildData>> GetAllGuildData()
    {
        using var ctx = GetDbContext(GuildDataDbName);
        return await ctx.Query<GuildData>()
            .ToListAsync();
    }

    public async Task InsertGuildDataIfNotExists(string platform, string guildId, string assignee)
    {
        using var ctx = GetDbContext(GuildDataDbName);

        var old = await GetGuildData(platform, guildId);
        if (old is not null) return;

        var user = new GuildData
        {
            Platform = platform,
            GuildId = guildId,
            Assignee = assignee
        };

        await ctx.InsertAsync(user);
    }

    public async Task UpdateGuildData(string platform, string guildId, string assignee)
    {
        using var ctx = GetDbContext(GuildDataDbName);

        var user = await GetGuildData(platform, guildId);
        if (user is null) return;

        user.Assignee = assignee;

        await ctx.UpdateAsync(user);
    }

    public async Task<UserData?> GetUserDataOrDefault(string platform, string userId)
    {
        using var ctx = GetDbContext(UserDataDbName);
        return await ctx.Query<UserData>()
            .Where(user => user.Platform == platform)
            .Where(user => user.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task SaveUserData(UserData userData)
    {
        using var dbCtx = GetDbContext(UserDataDbName);
        await dbCtx.InsertAsync(userData);
    }

    public async Task SaveCommandHistory(CommandHistory history)
    {
        using var dbCtx = GetDbContext(CommandHistoryDbName);
        await dbCtx.InsertAsync(history);
    }

    public async Task UpdateCommandHistory(Message message, string? response, bool isSucceeded)
    {
        using var ctx = GetDbContext(CommandHistoryDbName);
        var dt = message.Time.ToLocalTime();
        var history = await ctx.Query<CommandHistory>()
            .Where(history => history.CallTime == dt)
            .Where(history => history.UserId == message.Sender.UserId)
            .FirstAsync();

        history.RespondTime = DateTime.Now;
        history.Response = response;
        history.IsSucceeded = isSucceeded;
        await ctx.UpdateAsync(history);
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class YukiDatabaseAttribute : Attribute
{
    public string Name { get; set; }
    public Type[] TableTypes { get; set; }

    public YukiDatabaseAttribute(string dbName, params Type[] tableTypes)
    {
        Name = dbName.Replace(".db", "");
        TableTypes = tableTypes;
    }
}