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

public struct TileSpawner : IComponentData
{
    public float2 TileSize;
    public Entity TilePrefab;
}