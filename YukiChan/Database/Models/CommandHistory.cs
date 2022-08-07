using SQLite;

#pragma warning disable CS8618

namespace YukiChan.Database.Models;

[Table("command_history")]
public class CommandHistory
{
    /// <summary>
    ///     In milliseconds.
    /// </summary>
    [PrimaryKey]
    [Column("call_timestamp")]
    public long CallTimestamp { get; set; }

    [Column("reply_timestamp")] public long ReplyTimestamp { get; set; }

    [Column("context")] public string Context { get; set; }


    [Column("group_uin")] public uint GroupUin { get; set; }

    [Column("group_name")] public string GroupName { get; set; }

    [Column("user_uin")] public uint UserUin { get; set; }

    [Column("user_name")] public string UserName { get; set; }

    [Column("user_authority")] public YukiUserAuthority UserAuthority { get; set; }

    [Column("module_name")] public string ModuleName { get; set; }


    [Column("module_cmd")] public string ModuleCmd { get; set; }

    [Column("command_name")] public string CommandName { get; set; }

    [Column("command_cmd")] public string CommandCmd { get; set; }

    [Column("command_raw")] public string CommandRaw { get; set; }

    [Column("reply")] public string Reply { get; set; }
}