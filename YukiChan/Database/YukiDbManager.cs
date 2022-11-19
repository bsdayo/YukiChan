using System.Reflection;
using Chloe.Annotations;
using Chloe.SQLite;
using Chloe.SQLite.DDL;
using Flandre.Core.Utils;
using Microsoft.Data.Sqlite;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Database;

// [YukiDatabase(CommandHistoryDbName, typeof(CommandHistory))]
[YukiDatabase(GuildDataDbName, typeof(GuildData))]
public partial class YukiDbManager
{
    private readonly Logger _logger = new("Database");

    private const string CommandHistoryDbName = "command-history";
    private const string GuildDataDbName = "guilds";

    public YukiDbManager()
    {
        var attrs = GetType().GetCustomAttributes<YukiDatabaseAttribute>(false);

        foreach (var attr in attrs)
        {
            using var ctx = GetDbContext(attr.Name);
            _logger.Debug($"更新数据库 {YukiDir.Databases}/{attr.Name}.db");

            foreach (var tableType in attr.TableTypes)
            {
                var tableAttr = tableType.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttr is not null ? tableAttr.Name : tableType.Name;
                new SQLiteTableGenerator(ctx).CreateTable(tableType, tableName);
                _logger.Debug($"  通过类 {tableType.Name} 创建表 {tableName}");
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