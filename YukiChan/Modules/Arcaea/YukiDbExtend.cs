using YukiChan.Modules.Arcaea;
using YukiChan.Modules.Arcaea.Models;

// ReSharper disable CheckNamespace

namespace YukiChan.Database;

[YukiDatabase(ArcaeaDbName, typeof(ArcaeaDatabaseUser), typeof(ArcaeaGuessUser))]
public partial class YukiDbManager
{
    private const string ArcaeaDbName = "Arcaea";

    public void AddArcaeaUser(uint uin, string id, string name)
    {
        var user = new ArcaeaDatabaseUser
        {
            Uin = uin,
            Id = id,
            Name = name
        };

        Databases[ArcaeaDbName].InsertOrReplace(user, typeof(ArcaeaDatabaseUser));
    }

    public ArcaeaDatabaseUser? GetArcaeaUser(uint uin)
    {
        return Databases[ArcaeaDbName].FindWithQuery<ArcaeaDatabaseUser>(
            "SELECT * FROM arcaea_users WHERE uin = ?", uin);
    }

    public bool DeleteArcaeaUser(uint uin)
    {
        var user = GetArcaeaUser(uin);
        if (user is null) return false;
        Databases[ArcaeaDbName].Delete<ArcaeaDatabaseUser>(uin);
        return true;
    }

    public ArcaeaGuessUser? GetArcaeaGuessUserOfDate(uint groupUin, uint userUin, DateTime date)
    {
        return Databases[ArcaeaDbName].FindWithQuery<ArcaeaGuessUser>(
            "SELECT * FROM arcaea_guess WHERE group_uin = ? AND user_uin = ? AND year = ? AND month = ? AND day = ?",
            groupUin, userUin, date.Year, date.Month, date.Day);
    }

    public ArcaeaGuessUser? GetArcaeaGuessUserOfDate(uint userUin, DateTime date)
    {
        return Databases[ArcaeaDbName].FindWithQuery<ArcaeaGuessUser>(
            "SELECT * FROM arcaea_guess WHERE user_uin = ? AND year = ? AND month = ? AND day = ?",
            userUin, date.Year, date.Month, date.Day);
    }

    public List<ArcaeaGuessUser> GetArcaeaGuessUserOfAllTime(uint groupUin, uint userUin)
    {
        return Databases[ArcaeaDbName].Query<ArcaeaGuessUser>(
            "SELECT * FROM arcaea_guess WHERE group_uin = ? AND user_uin = ?", groupUin, userUin);
    }

    public List<ArcaeaGuessUser> GetArcaeaGuessUserOfAllTime(uint userUin)
    {
        return Databases[ArcaeaDbName].Query<ArcaeaGuessUser>(
            "SELECT * FROM arcaea_guess WHERE user_uin = ?", userUin);
    }

    public List<ArcaeaGuessUser> GetArcaeaGuessUsersOfDate(uint groupUin, DateTime date)
    {
        return Databases[ArcaeaDbName].Query<ArcaeaGuessUser>(
            "SELECT * FROM arcaea_guess WHERE group_uin = ? AND year = ? AND month = ? AND day = ?",
            groupUin, date.Year, date.Month, date.Day);
    }

    public List<ArcaeaGuessUser> GetArcaeaGuessUsersOfDate(DateTime date)
    {
        return Databases[ArcaeaDbName].Query<ArcaeaGuessUser>(
            "SELECT * FROM arcaea_guess WHERE year = ? AND month = ? AND day = ?",
            date.Year, date.Month, date.Day);
    }

    public List<ArcaeaGuessUser> GetArcaeaGuessUsersOfDate(DateTime startDate, DateTime endDate)
    {
        return Databases[ArcaeaDbName].Query<ArcaeaGuessUser>(
            @"SELECT * FROM arcaea_guess WHERE
                (year BETWEEN ? AND ?) AND
                (month BETWEEN ? AND ?) AND
                (day BETWEEN ? AND ?)",
            startDate.Year, endDate.Year,
            startDate.Month, endDate.Month,
            startDate.Day, endDate.Day);
    }

    public void AddArcaeaGuessCount(uint groupUin, string groupName, uint userUin, string userName,
        ArcaeaGuessMode guessMode,
        bool isCorrect)
    {
        var now = DateTime.Now;
        var guessUser = GetArcaeaGuessUserOfDate(groupUin, userUin, now);

        var addCount = (ArcaeaGuessUser user, bool correct, ArcaeaGuessMode mode) =>
        {
            if (correct)
                _ = mode switch
                {
                    ArcaeaGuessMode.Easy => user.EasyCorrectCount++,
                    ArcaeaGuessMode.Normal => user.NormalCorrectCount++,
                    ArcaeaGuessMode.Hard => user.HardCorrectCount++,
                    ArcaeaGuessMode.Flash => user.FlashCorrectCount++,
                    ArcaeaGuessMode.GrayScale => user.GrayScaleCorrectCount++,
                    ArcaeaGuessMode.Invert => user.InvertCorrectCount++,
                    _ => -1
                };
            else
                _ = mode switch
                {
                    ArcaeaGuessMode.Easy => user.EasyWrongCount++,
                    ArcaeaGuessMode.Normal => user.NormalWrongCount++,
                    ArcaeaGuessMode.Hard => user.HardWrongCount++,
                    ArcaeaGuessMode.Flash => user.FlashWrongCount++,
                    ArcaeaGuessMode.GrayScale => user.GrayScaleWrongCount++,
                    ArcaeaGuessMode.Invert => user.InvertWrongCount++,
                    _ => -1
                };
            return user;
        };

        if (guessUser is not null)
        {
            guessUser = addCount(guessUser, isCorrect, guessMode);
            Databases[ArcaeaDbName].Update(guessUser);
        }
        else
        {
            var newUser = new ArcaeaGuessUser
            {
                Year = now.Year,
                Month = now.Month,
                Day = now.Day,
                GroupUin = groupUin,
                GroupName = groupName,
                UserUin = userUin,
                UserName = userName
            };
            newUser = addCount(newUser, isCorrect, guessMode);
            Databases[ArcaeaDbName].Insert(newUser, typeof(ArcaeaGuessUser));
        }
    }
}