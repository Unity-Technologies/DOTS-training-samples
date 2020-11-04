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

public struct TileSpawner : IComponentData
{
    public float2 TileSize;
    public Entity TilePrefab;
}

[Flags]
public enum WallBits : byte
{
    Left   = (1 << 0),
    Right  = (1 << 1),
    Top    = (1 << 2),
    Bottom = (1 << 3)
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
