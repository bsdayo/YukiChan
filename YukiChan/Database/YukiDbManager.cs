using System.Reflection;
using Konata.Core.Message;
using SQLite;
using YukiChan.Core;
using YukiChan.Database.Models;
using YukiChan.Utils;

namespace YukiChan.Database;

[YukiDatabase(UserdataDbName, typeof(YukiUser), typeof(YukiGroup))]
[YukiDatabase(CommandHistoryDbName, typeof(CommandHistory))]
public partial class YukiDbManager
{
    public readonly Dictionary<string, SQLiteConnection> Databases = new();

    private const string UserdataDbName = "Userdata";
    private const string CommandHistoryDbName = "CommandHistory";

    public YukiDbManager()
    {
        var attrs = GetType().GetCustomAttributes(typeof(YukiDatabaseAttribute), false);

        foreach (var attr in attrs)
        {
            if (attr is not YukiDatabaseAttribute database) continue;

            Databases.Add(database.Name, new SQLiteConnection($"Databases/{database.Name}.db"));
            YukiLogger.Debug($"创建数据库 Databases/{database.Name}.db");

            foreach (var tableType in database.TableTypes)
            {
                Databases[database.Name].CreateTable(tableType);
                var tableAttr = tableType.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttr is not null ? tableAttr.Name : tableType.Name;
                YukiLogger.Debug($"  通过类 {tableType.Name} 创建表 {tableName}");
            }
        }
    }

    public YukiUser? GetUser(uint userUin)
    {
        return Databases[UserdataDbName].FindWithQuery<YukiUser>(
            "SELECT * FROM users WHERE uin = ?", userUin);
    }

    public YukiGroup? GetGroup(uint groupUin)
    {
        return Databases[UserdataDbName].FindWithQuery<YukiGroup>(
            "SELECT * FROM groups WHERE uin = ?", groupUin);
    }

    public void AddUser(uint uin, YukiUserAuthority authority = YukiUserAuthority.User)
    {
        var user = new YukiUser
        {
            Uin = uin,
            Authority = authority
        };
        Databases[UserdataDbName].Insert(user, typeof(YukiUser));
    }

    public void AddGroup(uint groupUin, uint assignee)
    {
        var group = new YukiGroup
        {
            Uin = groupUin,
            Assignee = assignee
        };
        Databases[UserdataDbName].Insert(group);
    }

    public void UpdateGroupAssignee(uint groupUin, uint assignee)
    {
        Databases[UserdataDbName].Execute(
            "UPDATE groups SET assignee = ? WHERE uin = ?", assignee, groupUin);
    }

    public void UpdateUser(YukiUser user)
    {
        Databases[UserdataDbName].Update(user);
    }

    public bool BanUser(uint userUin)
    {
        var user = GetUser(userUin);
        if (user is null) return false;
        user.Authority = YukiUserAuthority.Banned;
        Databases[UserdataDbName].Update(user);
        return true;
    }

    public void AddCommandHistory(DateTime callTime, DateTime replyTime, MessageStruct.SourceType sourceType,
        uint groupUin, string groupName,
        uint userUin, string userName, YukiUserAuthority authority,
        string moduleName, string moduleCmd,
        string commandName, string commandCmd, string commandRaw, string reply)
    {
        Databases[CommandHistoryDbName].Insert(new CommandHistory
        {
            CallTimestamp = new DateTimeOffset(callTime).ToUnixTimeMilliseconds(),
            ReplyTimestamp = new DateTimeOffset(replyTime).ToUnixTimeMilliseconds(),
            Context = sourceType.ToString(),

            GroupUin = groupUin,
            GroupName = groupName,
            UserUin = userUin,
            UserName = userName,
            UserAuthority = authority,

            ModuleName = moduleName,
            ModuleCmd = moduleCmd,
            CommandName = commandName,
            CommandCmd = commandCmd,
            CommandRaw = commandRaw,
            Reply = reply
        }, typeof(CommandHistory));
    }

    public int GetTotalCommandHistoryCount()
    {
        return Databases[CommandHistoryDbName].FindWithQuery<int>(
            "SELECT COUNT(*) FROM command_history");
    }

    public KeyValuePair<string, int>[] GetTodayCommandHistoryRank()
    {
        var today = DateTime.Today;
        var startT = new DateTimeOffset(
                new DateTime(today.Year, today.Month, today.Day, 0, 0, 0))
            .ToUnixTimeMilliseconds();
        var endT = new DateTimeOffset(
                new DateTime(today.Year, today.Month, today.Day, 23, 59, 59))
            .ToUnixTimeMilliseconds();

        var todayHistories = Databases[CommandHistoryDbName].Query<CommandHistory>(
            "SELECT * FROM command_history WHERE call_timestamp BETWEEN ? AND ?", startT, endT);

        var rank = new Dictionary<string, int>();

        foreach (var history in todayHistories)
        {
            var cmd = $"{Global.YukiConfig.CommandPrefix}{history.ModuleCmd} {history.CommandCmd}";
            if (rank.ContainsKey(cmd))
                rank[cmd]++;
            else
                rank[cmd] = 1;
        }

        return rank.OrderByDescending(r => r.Value).ToArray();
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