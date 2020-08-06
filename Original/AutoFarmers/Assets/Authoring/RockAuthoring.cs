using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct RockDataSpawner : IComponentData
{
    public Entity prefab;
    public Vector2Int mapSize;
    public int rockCount;
}

public struct Rock : IComponentData
{
    public Matrix4x4 matrix;

}

public struct RockRect : IComponentData
{
    public RectInt Value;
}

public struct Health : IComponentData
{
    public int Value;
}

public struct BatchNumber : IComponentData
{
    public int Value;
}

public struct BatchIndex : IComponentData
{
    public int Value;
}

public struct WorldMatrix : IComponentData
{
    public Matrix4x4 Value;
}

public struct StartHealth : IComponentData
{
    public int Value;
}
