using System.Reflection;
using SQLite;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Database;

[YukiDatabase(UserdataDbName, typeof(YukiUser), typeof(YukiGroup))]
public partial class YukiDbManager
{
    private readonly Dictionary<string, SQLiteConnection> _databases = new();

    private const string UserdataDbName = "Userdata";

    public YukiDbManager()
    {
        var attrs = GetType().GetCustomAttributes(typeof(YukiDatabaseAttribute), false);

        foreach (var attr in attrs)
        {
            if (attr is not YukiDatabaseAttribute database) continue;

            _databases.Add(database.Name, new SQLiteConnection($"Databases/{database.Name}.db"));
            YukiLogger.Debug($"创建数据库 Databases/{database.Name}.db");

            foreach (var tableType in database.TableTypes)
            {
                _databases[database.Name].CreateTable(tableType);
                var tableAttr = tableType.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttr is not null ? tableAttr.Name : tableType.Name;
                YukiLogger.Debug($"  通过类 {tableType.Name} 创建表 {tableName}");
            }
        }
    }

    public YukiUser? GetUser(uint userUin)
    {
        return _databases[UserdataDbName].FindWithQuery<YukiUser>(
            "SELECT * FROM users WHERE uin = ?", userUin);
    }

    public YukiGroup? GetGroup(uint groupUin)
    {
        return _databases[UserdataDbName].FindWithQuery<YukiGroup>(
            "SELECT * FROM groups WHERE uin = ?", groupUin);
    }

    public void AddUser(uint uin, YukiUserAuthority authority = YukiUserAuthority.User)
    {
        var user = new YukiUser
        {
            Uin = uin,
            Authority = authority
        };
        _databases[UserdataDbName].Insert(user, typeof(YukiUser));
    }

    public void AddGroup(uint groupUin)
    {
        var group = new YukiGroup
        {
            Uin = groupUin
        };
        _databases[UserdataDbName].Insert(group);
    }

    public void UpdateUser(YukiUser user)
    {
        _databases[UserdataDbName].Update(user);
    }

    public bool BanUser(uint userUin)
    {
        var user = GetUser(userUin);
        if (user is null) return false;
        user.Authority = YukiUserAuthority.Banned;
        _databases[UserdataDbName].Update(user);
        return true;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class YukiDatabaseAttribute : Attribute
{
    public string Name { get; set; }
    public Type[] TableTypes { get; set; }

    public YukiDatabaseAttribute(string dbName, params Type[] tableTypes)
    {
        Name = dbName.Replace(".db", "");
        TableTypes = tableTypes;
    }
}