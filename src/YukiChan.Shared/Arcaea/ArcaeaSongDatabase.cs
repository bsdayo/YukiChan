using Chloe;
using Chloe.SQLite;
using Microsoft.Data.Sqlite;
using YukiChan.Shared.Arcaea.Models;
using YukiChan.Shared.Utils;

namespace YukiChan.Shared.Arcaea;

public class ArcaeaSongDatabase
{
    private static readonly string DefaultPath = Path.Combine(YukiDir.ArcaeaAssets, "arcsong.db");

    public static ArcaeaSongDatabase Default { get; } = new(DefaultPath);

    private readonly string _dbPath;

    public ArcaeaSongDatabase(string dbPath) => _dbPath = dbPath;

    private IDbContext GetSongDbContext()
    {
        return new SQLiteContext(() => new SqliteConnection($"DataSource={_dbPath}"));
    }

    public async Task<List<ArcaeaSongDbAlias>> GetAliasesById(string songId)
    {
        using var ctx = GetSongDbContext();
        return await ctx.Query<ArcaeaSongDbAlias>()
            .Where(alias => alias.SongId == songId)
            .ToListAsync();
    }

    public async Task<List<ArcaeaSongDbChart>> GetChartsById(string songId)
    {
        using var ctx = GetSongDbContext();
        return await ctx.Query<ArcaeaSongDbChart>()
            .Where(chart => chart.SongId == songId)
            .ToListAsync();
    }

    public ArcaeaSongDbPackage? GetPackageBySet(string set)
    {
        using var ctx = GetSongDbContext();
        return ctx.Query<ArcaeaSongDbPackage>()
            .FirstOrDefault(package => package.Set == set);
    }

    public async Task<List<ArcaeaSongDbChart>> GetAllCharts()
    {
        using var ctx = GetSongDbContext();
        return await ctx.Query<ArcaeaSongDbChart>().ToListAsync();
    }

    public async Task<List<ArcaeaSongDbPackage>> GetAllPackages()
    {
        using var ctx = GetSongDbContext();
        return await ctx.Query<ArcaeaSongDbPackage>().ToListAsync();
    }

    public async Task<List<ArcaeaSongDbAlias>> GetAllAliases()
    {
        using var ctx = GetSongDbContext();
        return await ctx.Query<ArcaeaSongDbAlias>().ToListAsync();
    }

    /// <summary>
    /// 模糊搜索曲目，依赖 arcsong.db
    /// </summary>
    /// <param name="source">搜索文本</param>
    /// <returns>搜索到的曲目，若未搜索到返回 null</returns>
    public async Task<ArcaeaSong?> FuzzySearchSong(string source)
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
    public async Task<string?> FuzzySearchId(string source, IDbContext? songDbContext = null)
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

    public async Task InsertOrUpdateChart(ArcaeaSongDbChart chart)
    {
        using var ctx = GetSongDbContext();
        await ctx.Session.ExecuteNonQueryAsync("PRAGMA foreign_keys = OFF");
        await ctx.InsertOrUpdateAsync(chart);
    }

    public async Task InsertOrUpdatePackage(ArcaeaSongDbPackage package)
    {
        using var ctx = GetSongDbContext();
        await ctx.Session.ExecuteNonQueryAsync("PRAGMA foreign_keys = OFF");
        await ctx.InsertOrUpdateAsync(package);
    }

    public async Task InsertOrUpdateAlias(ArcaeaSongDbAlias alias)
    {
        using var ctx = GetSongDbContext();
        await ctx.Session.ExecuteNonQueryAsync("PRAGMA foreign_keys = OFF");
        await ctx.InsertOrUpdateAsync(alias);
    }

    public Task InsertOrUpdateAlias(string songId, string alias)
    {
        return InsertOrUpdateAlias(new ArcaeaSongDbAlias
        {
            Alias = alias,
            SongId = songId
        });
    }
}