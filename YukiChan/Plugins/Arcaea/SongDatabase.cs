using Chloe;
using Chloe.SQLite;
using Microsoft.Data.Sqlite;
using YukiChan.Plugins.Arcaea.Models;
using YukiChan.Utils;

namespace YukiChan.Plugins.Arcaea;

public static class ArcaeaSongDatabase
{
    private static readonly string DbPath = $"{YukiDir.ArcaeaAssets}/arcsong.db";

    private static readonly Lazy<IDbContext> SongDb = new(() =>
        new SQLiteContext(() => new SqliteConnection($"DataSource={DbPath}")));

    public static ArcaeaSongDbAlias? GetAliasById(string songId)
    {
        return SongDb.Value.Query<ArcaeaSongDbAlias>()
            .FirstOrDefault(alias => alias.SongId == songId);
    }

    public static void AddAlias(string songId, string alias)
    {
        SongDb.Value.Session.ExecuteNonQuery("PRAGMA foreign_keys = OFF");
        var dbAlias = new ArcaeaSongDbAlias
        {
            Alias = alias,
            SongId = songId
        };
        SongDb.Value.Insert(dbAlias);
    }

    public static List<ArcaeaSongDbChart> GetAllCharts()
    {
        return SongDb.Value.Query<ArcaeaSongDbChart>().ToList();
    }

    public static List<ArcaeaSongDbChart> GetChartsById(string songId)
    {
        return SongDb.Value.Query<ArcaeaSongDbChart>()
            .Where(chart => chart.SongId == songId)
            .ToList();
    }

    public static ArcaeaSongDbPackage? GetPackageBySet(string set)
    {
        return SongDb.Value.Query<ArcaeaSongDbPackage>()
            .FirstOrDefault(package => package.Set == set);
    }

    /// <summary>
    /// 模糊搜索曲目，依赖 arcsong.db
    /// </summary>
    /// <param name="source">搜索文本</param>
    /// <returns>搜索到的曲目，若未搜索到返回 null</returns>
    public static ArcaeaSong? FuzzySearchSong(string source)
    {
        var songId = FuzzySearchId(source);
        if (songId is null) return null;

        var charts = SongDb.Value.Query<ArcaeaSongDbChart>()
            .Where(chart => chart.SongId == songId)
            .ToList();
        charts.Sort((chartA, chartB) => chartA.RatingClass - chartB.RatingClass);

        return ArcaeaSong.FromDatabase(charts);
    }

    /// <summary>
    /// 模糊搜索曲目 ID，依赖 arcsong.db
    /// </summary>
    /// <param name="source">搜索文本</param>
    /// <returns>搜索到的曲目 ID，若未搜索到返回 null</returns>
    public static string? FuzzySearchId(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return null;

        source = source.Replace(" ", "").ToLower();

        var aliases = SongDb.Value.Query<ArcaeaSongDbAlias>()
            .Where(alias => alias.SongId == source || alias.Alias.ToLower() == source)
            .ToList();

        if (aliases.Count > 0)
            return aliases[0].SongId;

        var charts = SongDb.Value.Query<ArcaeaSongDbChart>().ToList();

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