using System.Reflection;
using SQLite;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Database;

[YukiDbTable(typeof(YukiUser))]
[YukiDbTable(typeof(YukiGroup))]
public partial class YukiDbManager
{
    protected readonly SQLiteConnection _database;

    public YukiDbManager()
    {
        _database = new SQLiteConnection("Database.db");

        var attrs = GetType().GetCustomAttributes(typeof(YukiDbTableAttribute), false);

        foreach (var attr in attrs)
        {
            if (attr is not YukiDbTableAttribute table) continue;

            _database.CreateTable(table.TableType);

            var tableAttr = table.TableType.GetCustomAttribute<TableAttribute>();
            var tableName = tableAttr is not null ? tableAttr.Name : table.TableType.Name;

            BotLogger.Debug($"通过类 {table.TableType.Name} 创建数据库表 {tableName}");
        }
    }

    public YukiUser? GetUser(uint userUin)
    {
        return _database.FindWithQuery<YukiUser>(
            "SELECT * FROM users WHERE uin = ?", userUin);
    }

    public YukiGroup? GetGroup(uint groupUin)
    {
        return _database.FindWithQuery<YukiGroup>(
            "SELECT * FROM groups WHERE uin = ?", groupUin);
    }

    public void AddUser(uint uin, YukiUserAuthority authority = YukiUserAuthority.User)
    {
        var user = new YukiUser
        {
            Uin = uin,
            Authority = authority
        };
        _database.Insert(user, typeof(YukiUser));
    }

    public void AddGroup(uint groupUin)
    {
        var group = new YukiGroup
        {
            Uin = groupUin
        };
        _database.Insert(group);
    }

    public void UpdateUser(YukiUser user)
    {
        _database.Update(user);
    }

    public bool BanUser(uint userUin)
    {
        var user = GetUser(userUin);
        if (user is null) return false;
        user.Authority = YukiUserAuthority.Banned;
        _database.Update(user);
        return true;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class YukiDbTableAttribute : Attribute
{
    public Type TableType { get; set; }

    public YukiDbTableAttribute(Type tableType)
    {
        TableType = tableType;
    }
}