// ReSharper disable InconsistentNaming

namespace YukiChan.Shared.Data;

public enum YukiErrorCode
{
    // Server (-xxx)
    Ok = 0,
    UserGotBanned = -1,
    BadRequest = -400,
    Unauthorized = -401,
    NotFound = -404,
    InternalServerError = -500,
    Unknown = -114514,

    // Arcaea (-1xxx)
    Arcaea_NotBound = -1001,
    Arcaea_InvalidUserCode = -1002,
    Arcaea_SongNotFound = -1003,
    Arcaea_NotPlayedRecently = -1004,
    Arcaea_AliasAlreadyExists = -1005,
    Arcaea_AliasSubmissionAlreadyExists = -1006,

    Arcaea_AuaErrorStart = -1100,
    Arcaea_AuaErrorEnd = -1400
}