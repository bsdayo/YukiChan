using YukiChan.Modules.Maimai.Models;

// ReSharper disable CheckNamespace

namespace YukiChan.Database;

[YukiDbTable(typeof(MaimaiUser))]
public partial class YukiDbManager
{
    internal void AddMaimaiUser(uint uin, string name)
    {
        var user = new MaimaiUser
        {
            Uin = uin,
            Name = name
        };

        _database.InsertOrReplace(user, typeof(MaimaiUser));
    }

    internal MaimaiUser? GetMaimaiUser(uint uin)
    {
        return _database.FindWithQuery<MaimaiUser>(
            "SELECT * FROM maimai_users WHERE uin = ?", uin);
    }

    internal bool DeleteMaimaiUser(uint uin)
    {
        var user = GetMaimaiUser(uin);
        if (user is null) return false;
        _database.Delete<MaimaiUser>(uin);
        return true;
    }
}