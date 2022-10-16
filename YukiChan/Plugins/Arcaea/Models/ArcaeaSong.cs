using ArcaeaUnlimitedAPI.Lib.Responses;
using Chloe.Annotations;

namespace YukiChan.Plugins.Arcaea.Models;

#pragma warning disable CS8618
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class ArcaeaSong
{
    public string SongId { get; private init; }

    public string Set { get; private init; }

    public string SetFriendly { get; private init; }

    public ArcaeaChart[] Difficulties { get; private init; }

    private ArcaeaSong()
    {
    }

    public static ArcaeaSong FromAua(AuaSongInfoContent content)
    {
        return new ArcaeaSong
        {
            SongId = content.SongId,
            Set = content.Difficulties[0].Set,
            SetFriendly = content.Difficulties[0].SetFriendly,
            Difficulties = content.Difficulties.Select((d, i) => new ArcaeaChart
            {
                RatingClass = i,
                NameEn = d.NameEn,
                NameJp = d.NameJp,
                Artist = d.Artist,
                Bpm = d.Bpm,
                BpmBase = d.BpmBase,
                Time = d.Time,
                Side = d.Side,
                WorldUnlock = d.WorldUnlock,
                RemoteDownload = d.RemoteDownload,
                Bg = d.Bg,
                Date = d.Date,
                Version = d.Version,
                Difficulty = d.Difficulty,
                Rating = d.Rating,
                Note = d.Note,
                ChartDesigner = d.ChartDesigner,
                JacketDesigner = d.JacketDesigner,
                JacketOverride = d.JacketOverride,
                AudioOverride = d.AudioOverride
            }).ToArray()
        };
    }

    public static ArcaeaSong FromDatabase(List<ArcaeaSongDbChart> charts)
    {
        return new ArcaeaSong
        {
            SongId = charts[0].SongId,
            Set = charts[0].Set,
            SetFriendly = ArcaeaSongDatabase.GetPackageBySet(charts[0].Set)!.Name,
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

[Table("charts")]
public class ArcaeaSongDbChart : ArcaeaChart
{
    [Column("song_id", IsPrimaryKey = true)]
    public string SongId { get; init; }

    [Column("set")] public string Set { get; init; }
}

public class ArcaeaChart
{
    [Column("rating_class", IsPrimaryKey = true)]
    public int RatingClass { get; init; }

    [Column("name_en")] public string NameEn { get; init; }

    [Column("name_jp")] public string NameJp { get; init; }

    [Column("artist")] public string Artist { get; init; }

    [Column("bpm")] public string Bpm { get; init; }

    [Column("bpm_base")] public double BpmBase { get; init; }

    [Column("time")] public int Time { get; init; }

    [Column("side")] public int Side { get; init; }

    [Column("world_unlock")] public bool WorldUnlock { get; init; }

    [Column("remote_download")] public bool RemoteDownload { get; init; }

    [Column("bg")] public string Bg { get; init; }

    [Column("date")] public long Date { get; init; }

    [Column("version")] public string Version { get; init; }

    [Column("difficulty")] public int Difficulty { get; init; }

    [Column("rating")] public int Rating { get; init; }

    [Column("note")] public int Note { get; init; }

    [Column("chart_designer")] public string ChartDesigner { get; init; }

    [Column("jacket_designer")] public string JacketDesigner { get; init; }

    [Column("jacket_override")] public bool JacketOverride { get; init; }

    [Column("audio_override")] public bool AudioOverride { get; init; }
}