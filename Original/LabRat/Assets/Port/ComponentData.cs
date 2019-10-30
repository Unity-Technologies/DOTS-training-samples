using Unity.Entities;

public struct Speed : IComponentData
{
    public float Value;
}

public enum eDirection : byte
{
    North,
    South,
    East,
    West
}

public struct Direction : IComponentData
{
    public eDirection Value;
}

public struct MouseTag : IComponentData
{
}

public struct CatTag : IComponentData
{
}

public struct Falling : IComponentData
{
    public float Timer;
}
