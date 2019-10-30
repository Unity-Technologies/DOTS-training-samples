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
    West
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
