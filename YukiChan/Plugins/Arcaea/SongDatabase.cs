using Chloe;
using Chloe.SQLite;
using Microsoft.Data.Sqlite;
using YukiChan.Plugins.Arcaea.Models;
using YukiChan.Utils;

namespace YukiChan.Plugins.Arcaea;

public static class ArcaeaSongDatabase
{
    private static readonly string DbPath = $"{YukiDir.ArcaeaAssets}/arcsong.db";

    private static IDbContext GetSongDbContext()
    {
        return new SQLiteContext(() => new SqliteConnection($"DataSource={DbPath}"));
    }

    public static ArcaeaSongDbAlias? GetAliasById(string songId)
    {
        using var ctx = GetSongDbContext();
        return ctx.Query<ArcaeaSongDbAlias>()
            .FirstOrDefault(alias => alias.SongId == songId);
    }

    public static void AddAlias(string songId, string alias)
    {
        using var ctx = GetSongDbContext();
        ctx.Session.ExecuteNonQuery("PRAGMA foreign_keys = OFF");
        var dbAlias = new ArcaeaSongDbAlias
        {
            Alias = alias,
            SongId = songId
        };
        ctx.Insert(dbAlias);
    }

    public static List<ArcaeaSongDbChart> GetAllCharts()
    {
        using var ctx = GetSongDbContext();
        return ctx.Query<ArcaeaSongDbChart>().ToList();
    }

    public static List<ArcaeaSongDbChart> GetChartsById(string songId)
    {
        using var ctx = GetSongDbContext();
        return ctx.Query<ArcaeaSongDbChart>()
            .Where(chart => chart.SongId == songId)
            .ToList();
    }

    public static ArcaeaSongDbPackage? GetPackageBySet(string set)
    {
        using var ctx = GetSongDbContext();
        return ctx.Query<ArcaeaSongDbPackage>()
            .FirstOrDefault(package => package.Set == set);
    }

    /// <summary>
    /// 模糊搜索曲目，依赖 arcsong.db
    /// </summary>
    /// <param name="source">搜索文本</param>
    /// <returns>搜索到的曲目，若未搜索到返回 null</returns>
    public static async Task<ArcaeaSong?> FuzzySearchSong(string source)
    {
        using var ctx = GetSongDbContext();

        var songId = await FuzzySearchId(source, ctx);
        if (songId is null) return null;

        var charts = await ctx.Query<ArcaeaSongDbChart>()
            .Where(chart => chart.SongId == songId)
            .ToListAsync();
        charts.Sort((chartA, chartB) => chartA.RatingClass - chartB.RatingClass);

        return ArcaeaSong.FromDatabase(charts);
    }

    /// <summary>
    /// 模糊搜索曲目 ID，依赖 arcsong.db
    /// </summary>
    /// <param name="source">搜索文本</param>
    /// <param name="songDbContext"></param>
    /// <returns>搜索到的曲目 ID，若未搜索到返回 null</returns>
    public static async Task<string?> FuzzySearchId(string source, IDbContext? songDbContext = null)
    {
        if (string.IsNullOrWhiteSpace(source))
            return null;

        var ctx = songDbContext ?? GetSongDbContext();

        source = source.Replace(" ", "").ToLower();

        var aliases = await ctx.Query<ArcaeaSongDbAlias>()
            .Where(alias => alias.SongId == source || alias.Alias.ToLower() == source)
            .ToListAsync();

        if (aliases.Count > 0)
            return aliases[0].SongId;

        var charts = await ctx.Query<ArcaeaSongDbChart>().ToListAsync();

        if (songDbContext is null) ctx.Dispose();

        return (
                charts.FirstOrDefault(chart => chart.SongId == source) ??
                charts.FirstOrDefault(chart => chart.NameEn.RemoveString(" ").ToLower() == source ||
                                               chart.NameJp.RemoveString(" ").ToLower() == source) ??
                charts.FirstOrDefault(chart => source.Length > 1 &&
                                               chart.NameEn.GetAbbreviation().ToLower() == source) ??
                charts.FirstOrDefault(chart => source.Length > 4 &&
                                               chart.NameEn.RemoveString(" ").ToLower().Contains(source)))
            ?.SongId;
    }
}