using SQLite;
using YukiChan.Database.Models;

namespace YukiChan.Database;

public class YukiDbManager
{
    private readonly SQLiteConnection _database;

    public YukiDbManager()
    {
        _database = new SQLiteConnection("Database.db");
        _database.CreateTable<YukiUser>();
        _database.CreateTable<YukiGroup>();
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

    public void AddUser(YukiUser user)
    {
        _database.Insert(user);
    }
    
    public void AddGroup(YukiGroup group)
    {
        _database.Insert(group);
    }
}