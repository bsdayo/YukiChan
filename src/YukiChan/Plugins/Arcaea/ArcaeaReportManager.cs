using System.Text.Json;
using Chloe;
using Chloe.Annotations;
using Chloe.SQLite;
using Chloe.SQLite.DDL;
using Microsoft.Data.Sqlite;
using YukiChan.Shared;
using YukiChan.Shared.Arcaea.Models;

namespace YukiChan.Plugins.Arcaea;

public class ArcaeaReportManager
{
    private static IDbContext GetDbContext(string usercode)
    {
        var path = Path.Combine(YukiDir.ArcaeaData, "best30", $"{usercode}.db");
        var ctx = new SQLiteContext(() => new SqliteConnection($"DataSource={path}"));
        new SQLiteTableGenerator(ctx).CreateTable(typeof(ArcaeaReportBest30Data), "best30");
        return ctx;
    }

    public async Task SaveUserBest30(ArcaeaBest30 best30)
    {
        var json = JsonSerializer.Serialize(best30);
        using var ctx = GetDbContext(best30.User.Code);
        await ctx.InsertAsync(new ArcaeaReportBest30Data
        {
            DateTime = DateTime.Now,
            Best30Json = json
        });
    }
}

[Table("best30")]
public class ArcaeaReportBest30Data
{
    [Column("id", IsPrimaryKey = true)]
    [AutoIncrement]
    public int Id { get; set; }

    [Column("datetime")]
    public required DateTime DateTime { get; set; }

    [Column("best30_json")]
    public required string Best30Json { get; set; }
}