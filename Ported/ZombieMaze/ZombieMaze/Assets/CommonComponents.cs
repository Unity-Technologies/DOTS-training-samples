using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using System;

public struct Speed : IComponentData
{
    public float Value;

    public Speed(float speed)
    {
        Value = speed;
    }
}

public struct CapsuleRotation : IComponentData
{
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
    public int2 MazeSize;
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

public struct MazeSize : IComponentData
{
    public int2 Value;
}

[Flags]
public enum WallBits : byte
{
    Left   = (1 << 0),
    Right  = (1 << 1),
    Top    = (1 << 2),
    Bottom = (1 << 3),
    Visited = (1 << 4)
}

public struct TagDijkstraGenerateMap : IComponentData {};

public struct MapCell : IBufferElementData
{
    public byte Value;
}

public struct DistCell : IBufferElementData
{
    public int Value;
}

public struct DijkstraMap : IComponentData
{
    public int Width;
    public int Height;

    public DijkstraMap(int width, int height)
    {
        Width  = width;
        Height = height;
    }
}
