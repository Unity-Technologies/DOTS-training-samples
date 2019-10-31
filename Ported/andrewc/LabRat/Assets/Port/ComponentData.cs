using Unity.Entities;
using Unity.Mathematics;

public enum eColor : byte
{
    None,
    Black,
    Red,
    Blue,
    Green
}

public enum eDirection : byte
{
    North,
    South,
    East,
    West,

    Invalid = 0xff,
}

public static class DirectionExtensions
{
    public static float3 GetWorldDirection(this eDirection direction)
    {
        switch (direction)
        {
            case eDirection.North:
                return new float3(0.0f, 0.0f, 1.0f);

            case eDirection.South:
                return new float3(0.0f, 0.0f, -1.0f);

            case eDirection.West:
                return new float3(-1.0f, 0.0f, 0.0f);

            case eDirection.East:
                return new float3(1.0f, 0.0f, 0.0f);
        }

        return new float3(0.0f, 1.0f, 0.0f);
    }

    public static eDirection Flip(this eDirection direction)
    {
        switch (direction)
        {
            case eDirection.North:
                return eDirection.South;

            case eDirection.South:
                return eDirection.North;

            case eDirection.West:
                return eDirection.East;

            case eDirection.East:
                return eDirection.West;
        }

        return eDirection.Invalid;
    }

    public static eDirection RotateClockwise(this eDirection direction)
    {
        switch (direction)
        {
            case eDirection.North:
                return eDirection.East;

            case eDirection.South:
                return eDirection.West;

            case eDirection.West:
                return eDirection.North;

            case eDirection.East:
                return eDirection.South;
        }

        return eDirection.Invalid;
    }

    public static eDirection RotateCounterClockwise(this eDirection direction)
    {
        switch (direction)
        {
            case eDirection.North:
                return eDirection.West;

            case eDirection.South:
                return eDirection.East;

            case eDirection.West:
                return eDirection.South;

            case eDirection.East:
                return eDirection.North;
        }

        return eDirection.Invalid;
    }

    public static bool IsVertical(this eDirection direction)
    {
        return direction == eDirection.North || direction == eDirection.South;
    }
}

public enum eTileType : byte
{
    Blank,
    DirectionArrow,
    Hole,
    Confuse,
    HomeBase
}

public struct Speed : IComponentData
{
    public float Value;
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

public struct Spawner_FromEntity : IComponentData
{
    public Entity RatPrefab;
    public float RatFrequency;
    public float RatMaxSpawn;

    public Entity CatPrefab;
    public float CatFrequency;
    public int CatMaxSpawn;

    public float3 SpawnPos;

    // Runtime Data
    public int RatSpawned;
    public int CatSpawned;
    public float RatCounter;
    public float CatCounter;
}
