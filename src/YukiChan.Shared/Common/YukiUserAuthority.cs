namespace YukiChan.Shared.Common;

public enum YukiUserAuthority
{
    Banned = -1,
    User = 0,
    Admin = 1,
    Master = 2
}

[AttributeUsage(AttributeTargets.Method)]
public class YukiUserAuthorityAttribute : Attribute
{
    public YukiUserAuthority RequiredAuthority { get; }

    public YukiUserAuthorityAttribute(YukiUserAuthority requiredAuthority)
    {
        RequiredAuthority = requiredAuthority;
    }
}