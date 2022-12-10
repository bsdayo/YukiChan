using ArcaeaUnlimitedAPI.Lib.Responses;
using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.Plugins.Arcaea.Factories;

public static class ArcaeaSongFactory
{
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
}