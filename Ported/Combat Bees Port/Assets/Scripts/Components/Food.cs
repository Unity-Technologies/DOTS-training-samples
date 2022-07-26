using Unity.Entities;
using Unity.Mathematics;

public struct Food : IComponentData
{
    public Entity target;

    public float3 targetPos;
}