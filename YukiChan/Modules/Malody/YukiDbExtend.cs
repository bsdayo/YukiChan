using YukiChan.Modules.Malody;

// ReSharper disable CheckNamespace

namespace YukiChan.Database;

[YukiDatabase(MalodyDbName, typeof(MalodyDatabaseUser))]
public partial class YukiDbManager
{
    private const string MalodyDbName = "Malody";

    public void AddMalodyUser(uint uin, int id, string name)
    {
        var user = new MalodyDatabaseUser
        {
            Uin = uin,
            Id = id,
            Name = name
        };

        Databases[MalodyDbName].InsertOrReplace(user, typeof(MalodyDatabaseUser));
    }

    public MalodyDatabaseUser? GetMalodyUser(uint uin)
    {
        return Databases[MalodyDbName].FindWithQuery<MalodyDatabaseUser>(
            "SELECT * FROM malody_users WHERE uin = ?", uin);
    }

    public bool DeleteMalodyUser(uint uin)
    {
        var user = GetMalodyUser(uin);
        if (user is null) return false;
        Databases[MalodyDbName].Delete<MalodyDatabaseUser>(uin);
        return true;
    }
}