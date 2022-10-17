using System.Reflection;
using Chloe.Annotations;
using Chloe.SQLite;
using Chloe.SQLite.DDL;
using Flandre.Core.Utils;
using Microsoft.Data.Sqlite;
using YukiChan.Utils;

namespace YukiChan.Database;

// [YukiDatabase(CommandHistoryDbName, typeof(CommandHistory))]
public partial class YukiDbManager
{
    private readonly Logger _logger = new("Database");

    private const string CommandHistoryDbName = "command-history";

    public YukiDbManager()
    {
        var attrs = GetType().GetCustomAttributes<YukiDatabaseAttribute>(false);

        foreach (var attr in attrs)
        {
            using var ctx = GetDbContext(attr.Name);
            _logger.Debug($"更新数据库 {YukiDir.Databases}/{attr.Name}.db");

            foreach (var tableType in attr.TableTypes)
            {
                var tableAttr = tableType.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttr is not null ? tableAttr.Name : tableType.Name;
                new SQLiteTableGenerator(ctx).CreateTable(tableType, tableName);
                _logger.Debug($"  通过类 {tableType.Name} 创建表 {tableName}");
            }
        }
    }

    private SQLiteContext GetDbContext(string dbName)
    {
        return new(() => new SqliteConnection($"DataSource={YukiDir.Databases}/{dbName}.db"));
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