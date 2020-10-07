using Unity.Entities;

public struct BotRole : IComponentData
{
    public Role Value;
}

public enum Role
{
    None,
    Scooper,
    PassFull,
    Thrower,
    PassEmpty,
    BriansSpecialTestRole
}