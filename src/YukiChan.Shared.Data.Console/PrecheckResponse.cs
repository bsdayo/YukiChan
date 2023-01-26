using YukiChan.Shared.Models;

namespace YukiChan.Shared.Data.Console;

public sealed class PrecheckResponse
{
    public required bool IsAssignee { get; init; }

    public required YukiUserAuthority UserAuthority { get; init; }
}