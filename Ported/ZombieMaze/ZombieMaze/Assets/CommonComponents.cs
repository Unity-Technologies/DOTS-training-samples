using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct Speed : IComponentData
{
    public float Value;

    public Speed(float speed)
    {
        Value = speed;
    }
}

public struct Position : IComponentData
{
    public float2 Value;
}

public struct Direction : IComponentData
{
    public float2 Value;
}

public struct Random : IComponentData
{
    public Unity.Mathematics.Random Value;

    public Random(uint seed)
    {
        Value = new Unity.Mathematics.Random(seed);
    }
}

public struct ZombieTag : IComponentData {}

public struct PlayerTag : IComponentData {}

public struct Spawner : IComponentData
{
    public uint2 MazeSize;
    public Entity Prefab;
}

public struct TileSpawner : IComponentData
{
}

public struct ZombieSpawner : IComponentData
{
    public uint NumZombies;
}

public struct MazeSpawner : IComponentData
{
    public uint OpenStripsWidth;
    public uint MazeStripsWidth;
}