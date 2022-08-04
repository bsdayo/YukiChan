using YukiChan.Modules.Arcaea.Models;

// ReSharper disable CheckNamespace

namespace YukiChan.Database;

[YukiDbTable(typeof(ArcaeaDatabaseUser))]
public partial class YukiDbManager
{
    internal void AddArcaeaUser(uint uin, string id, string name)
    {
        var user = new ArcaeaDatabaseUser
        {
            Uin = uin,
            Id = id,
            Name = name
        };

        _database.InsertOrReplace(user, typeof(ArcaeaDatabaseUser));
    }

    internal ArcaeaDatabaseUser? GetArcaeaUser(uint uin)
    {
        return _database.FindWithQuery<ArcaeaDatabaseUser>(
            "SELECT * FROM arcaea_users WHERE uin = ?", uin);
    }

    internal bool DeleteArcaeaUser(uint uin)
    {
        var user = GetArcaeaUser(uin);
        if (user is null) return false;
        _database.Delete<ArcaeaDatabaseUser>(uin);
        return true;
    }
}