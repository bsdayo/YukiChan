using Microsoft.EntityFrameworkCore;
using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.Shared.Utils;

public class ArcaeaSongDbContext : DbContext
{
    private readonly string _dbPath = "assets/arcaea/arcsong.db";

    public DbSet<ArcaeaSongDbChart> Charts => Set<ArcaeaSongDbChart>();

    public DbSet<ArcaeaSongDbAlias> Aliases => Set<ArcaeaSongDbAlias>();

    public DbSet<ArcaeaSongDbPackage> Packages => Set<ArcaeaSongDbPackage>();

    public ArcaeaSongDbContext()
    {
    }

    public ArcaeaSongDbContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ArcaeaSongDbChart>()
            .HasKey(nameof(ArcaeaSongDbChart.SongId), nameof(ArcaeaSongDbChart.RatingClass));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"DataSource={_dbPath}");

    /// <summary>
    /// 模糊搜索曲目，依赖 arcsong.db
    /// </summary>
    /// <param name="source">搜索文本</param>
    /// <returns>搜索到的曲目，若未搜索到返回 null</returns>
    public async Task<ArcaeaSong?> FuzzySearchSong(string source)
    {
        var songId = await FuzzySearchId(source);
        if (songId is null) return null;

        var charts = await Charts
            // .AsNoTracking()
            .Where(chart => chart.SongId == songId)
            .OrderBy(chart => chart.RatingClass)
            .ToListAsync();

        var set = charts[0].Set;
        var packageName = (await Packages
            .AsNoTracking()
            .FirstAsync(package => package.Set == set)).Name;

        return ArcaeaSong.FromDatabase(charts, packageName);
    }

    /// <summary>
    /// 模糊搜索曲目 ID，依赖 arcsong.db
    /// </summary>
    /// <param name="source">搜索文本</param>
    /// <returns>搜索到的曲目 ID，若未搜索到返回 null</returns>
    public async Task<string?> FuzzySearchId(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return null;

        source = source.Replace(" ", "").ToLower();

        var aliases = await Aliases
            .AsNoTracking()
            .Where(alias => alias.SongId == source || alias.Alias.ToLower() == source)
            .ToListAsync();

        if (aliases.Count > 0)
            return aliases[0].SongId;

        var charts = await Charts.AsNoTracking().ToArrayAsync();

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