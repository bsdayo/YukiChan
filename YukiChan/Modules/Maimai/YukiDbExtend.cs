using YukiChan.Modules.Maimai.Models;

// ReSharper disable CheckNamespace

namespace YukiChan.Database;

[YukiDatabase(MaimaiDbName, typeof(MaimaiUser))]
public partial class YukiDbManager
{
    private const string MaimaiDbName = "Maimai";

    internal void AddMaimaiUser(uint uin, string name)
    {
        var user = new MaimaiUser
        {
            Uin = uin,
            Name = name
        };

        _databases[MaimaiDbName].InsertOrReplace(user, typeof(MaimaiUser));
    }

    internal MaimaiUser? GetMaimaiUser(uint uin)
    {
        return _databases[MaimaiDbName].FindWithQuery<MaimaiUser>(
            "SELECT * FROM maimai_users WHERE uin = ?", uin);
    }

    internal bool DeleteMaimaiUser(uint uin)
    {
        var user = GetMaimaiUser(uin);
        if (user is null) return false;
        _databases[MaimaiDbName].Delete<MaimaiUser>(uin);
        return true;
    }
}