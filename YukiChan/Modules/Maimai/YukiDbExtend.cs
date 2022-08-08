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

        Databases[MaimaiDbName].InsertOrReplace(user, typeof(MaimaiUser));
    }

    internal MaimaiUser? GetMaimaiUser(uint uin)
    {
        return Databases[MaimaiDbName].FindWithQuery<MaimaiUser>(
            "SELECT * FROM maimai_users WHERE uin = ?", uin);
    }

    internal bool DeleteMaimaiUser(uint uin)
    {
        var user = GetMaimaiUser(uin);
        if (user is null) return false;
        Databases[MaimaiDbName].Delete<MaimaiUser>(uin);
        return true;
    }
}