using YukiChan.Shared.Database;

namespace YukiChan.Shared.Models.Arcaea;

#pragma warning disable CS8618
// ReSharper disable UnusedAutoPropertyAccessor.Global
public sealed class ArcaeaSong
{
    public string SongId { get; init; }

    public string Set { get; init; }

    public string SetFriendly { get; init; }

    public ArcaeaChart[] Difficulties { get; init; }

    public static ArcaeaSong FromDatabase(List<ArcaeaSongDbChart> charts)
    {
        return new ArcaeaSong
        {
            SongId = charts[0].SongId,
            Set = charts[0].Set,
            SetFriendly = ArcaeaSongDatabase.Default.GetPackageBySet(charts[0].Set)!.Name,
            Difficulties = charts.Select(c => new ArcaeaChart
            {
                RatingClass = c.RatingClass,
                NameEn = c.NameEn,
                NameJp = c.NameJp,
                Artist = c.Artist,
                Bpm = c.Bpm,
                BpmBase = c.BpmBase,
                Time = c.Time,
                Side = c.Side,
                WorldUnlock = c.WorldUnlock,
                RemoteDownload = c.RemoteDownload,
                Bg = c.Bg,
                Date = c.Date,
                Version = c.Version,
                Difficulty = c.Difficulty,
                Rating = c.Rating,
                Note = c.Note,
                ChartDesigner = c.ChartDesigner,
                JacketDesigner = c.JacketDesigner,
                JacketOverride = c.JacketOverride,
                AudioOverride = c.AudioOverride
            }).ToArray()
        };
    }
}