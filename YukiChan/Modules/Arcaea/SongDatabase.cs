using SQLite;
using YukiChan.Modules.Arcaea.Models;
using YukiChan.Utils;

namespace YukiChan.Modules.Arcaea;

public static class ArcaeaSongDatabase
{
    private static readonly Lazy<SQLiteConnection> SongDb = new(() =>
        new SQLiteConnection("Assets/Arcaea/arcsong.db"));

    public static bool Exists()
    {
        return File.Exists("Assets/Arcaea/arcsong.db");
    }

    public static List<ArcaeaSongDbAlias> GetAllAliases()
    {
        return SongDb.Value.Query<ArcaeaSongDbAlias>("SELECT * FROM alias");
    }

    public static void AddAlias(string songId, string alias)
    {
        var dbAlias = new ArcaeaSongDbAlias
        {
            Alias = alias,
            SongId = songId
        };
        SongDb.Value.InsertOrReplace(dbAlias);
    }

    public static List<ArcaeaSongDbChart> GetAllCharts()
    {
        return SongDb.Value.Query<ArcaeaSongDbChart>("SELECT * FROM charts");
    }

    public static List<ArcaeaSongDbChart> GetChartsById(string songId)
    {
        return SongDb.Value.Query<ArcaeaSongDbChart>(
            "SELECT * FROM charts WHERE song_id = ?", songId);
    }

    public static ArcaeaSongDbPackage? GetPackageBySet(string set)
    {
        return SongDb.Value.FindWithQuery<ArcaeaSongDbPackage>(
            "SELECT * FROM packages WHERE id = ?", set);
    }

    /// <summary>
    ///     模糊搜索曲目，依赖 arcsong.db
    /// </summary>
    /// <param name="source">搜索文本</param>
    /// <returns>搜索到的曲目，若未搜索到返回 null</returns>
    public static ArcaeaSong? FuzzySearchSong(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return null;

        source = source.RemoveString(" ").ToLower();
        List<ArcaeaSongDbChart> charts = new();

        var aliases = GetAllAliases()
            .Where(alias => alias.SongId == source ||
                            alias.Alias.ToLower() == source)
            .ToArray();

        if (aliases.Length > 0)
            charts = GetChartsById(aliases[0].SongId);

        if (charts.Count == 0)
        {
            var allCharts = GetAllCharts();
            charts = allCharts.FindAll(chart => chart.SongId == source);
            if (charts.Count == 0)
                charts = allCharts.FindAll(chart => chart.NameEn.RemoveString(" ").ToLower() == source ||
                                                    chart.NameJp.RemoveString(" ").ToLower() == source);
            if (charts.Count == 0)
                charts = allCharts.FindAll(chart => chart.NameEn.GetAbbreviation().ToLower() == source);
            if (charts.Count == 0)
                charts = allCharts.FindAll(chart => source.Length > 4 &&
                                                    chart.NameEn.RemoveString(" ").ToLower().Contains(source));
            if (charts.Count == 0) return null;
        }

        charts.Sort((chartA, chartB) => chartA.RatingClass - chartB.RatingClass);

        return ArcaeaSong.FromDatabase(charts);
    }

    /// <summary>
    ///     模糊搜索曲目 ID，依赖 arcsong.db
    /// </summary>
    /// <param name="source">搜索文本</param>
    /// <returns>搜索到的曲目 ID，若未搜索到返回 null</returns>
    public static string? FuzzySearchId(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return null;

        source = source.Replace(" ", "").ToLower();

        var aliases = GetAllAliases()
            .Where(alias => alias.SongId == source ||
                            alias.Alias.ToLower() == source)
            .ToArray();

        if (aliases.Length > 0)
            return aliases[0].SongId;

        var allCharts = GetAllCharts().ToArray();

        return (
                allCharts.FirstOrDefault(chart => chart.SongId == source) ??
                allCharts.FirstOrDefault(chart => chart.NameEn.RemoveString(" ").ToLower() == source ||
                                                  chart.NameJp.RemoveString(" ").ToLower() == source) ??
                allCharts.FirstOrDefault(chart => chart.NameEn.GetAbbreviation().ToLower() == source) ??
                allCharts.FirstOrDefault(chart => source.Length > 4 &&
                                                  chart.NameEn.RemoveString(" ").ToLower().Contains(source)))
            ?.SongId;
    }
}