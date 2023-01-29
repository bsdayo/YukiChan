namespace YukiChan.Shared.Models.Arcaea;

#pragma warning disable CS8618
// ReSharper disable UnusedAutoPropertyAccessor.Global
public sealed class ArcaeaSong
{
    public string SongId { get; init; }

    public string Set { get; init; }

    public string SetFriendly { get; init; }

    public ArcaeaChart[] Difficulties { get; init; }

    public static ArcaeaSong FromDatabase(List<ArcaeaSongDbChart> charts, string packageName)
    {
        return new ArcaeaSong
        {
            SongId = charts[0].SongId,
            Set = charts[0].Set,
            SetFriendly = packageName,
            Difficulties = charts.Select(c => new ArcaeaChart
            {
                Difficulty = (ArcaeaDifficulty)c.RatingClass,
                NameEn = c.NameEn,
                NameJp = c.NameJp,
                Artist = c.Artist,
                Bpm = c.Bpm,
                BpmBase = c.BpmBase,
                Time = c.Time,
                Side = c.Side,
                WorldUnlock = c.WorldUnlock,
                RemoteDownload = c.RemoteDownload,
                Background = c.Bg,
                Date = c.Date,
                Version = c.Version,
                Rating = c.Rating / 10d,
                Note = c.Note,
                ChartDesigner = c.ChartDesigner,
                JacketDesigner = c.JacketDesigner,
                JacketOverride = c.JacketOverride,
                AudioOverride = c.AudioOverride
            }).ToArray()
        };
    }
}