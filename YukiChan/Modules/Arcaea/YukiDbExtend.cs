using YukiChan.Modules.Arcaea.Models;

// ReSharper disable CheckNamespace

namespace YukiChan.Database;

[YukiDbTable(typeof(ArcaeaUser))]
public partial class YukiDbManager
{
    internal void AddArcaeaUser(uint uin, string id, string name)
    {
        var user = new ArcaeaUser
        {
            Uin = uin,
            Id = id,
            Name = name
        };

        _database.InsertOrReplace(user, typeof(ArcaeaUser));
    }

    internal ArcaeaUser? GetArcaeaUser(uint uin)
    {
        return _database.FindWithQuery<ArcaeaUser>(
            "SELECT * FROM arcaea_users WHERE uin = ?", uin);
    }

    internal bool DeleteArcaeaUser(uint uin)
    {
        var user = GetArcaeaUser(uin);
        if (user is null) return false;
        _database.Delete<ArcaeaUser>(uin);
        return true;
    }
}