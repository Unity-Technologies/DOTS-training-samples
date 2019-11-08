using Unity.Entities;
using Unity.Mathematics;

public struct BoardDataComponent : IComponentData
{
    public int2 size;
    public float2 cellSize;
}