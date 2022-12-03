using System.Reflection;
using Chloe.Annotations;
using Chloe.SQLite;
using Chloe.SQLite.DDL;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Database;

// [YukiDatabase(CommandHistoryDbName, typeof(CommandHistory))]
[YukiDatabase(GuildDataDbName, typeof(GuildData))]
public partial class YukiDbManager
{
    public ILogger<YukiDbManager> Logger { get; }

    private const string CommandHistoryDbName = "command-history";
    private const string GuildDataDbName = "guilds";

    public YukiDbManager(ILogger<YukiDbManager> logger)
    {
        Logger = logger;

        var attrs = GetType().GetCustomAttributes<YukiDatabaseAttribute>(false);

        foreach (var attr in attrs)
        {
            using var ctx = GetDbContext(attr.Name);
            Logger.LogDebug("更新数据库 {DbPath}/{DbName}.db",
                YukiDir.Databases, attr.Name);

            foreach (var tableType in attr.TableTypes)
            {
                var tableAttr = tableType.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttr is not null ? tableAttr.Name : tableType.Name;
                new SQLiteTableGenerator(ctx).CreateTable(tableType, tableName);
                Logger.LogDebug("  通过类 {ClassName} 创建表 {TableName}",
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