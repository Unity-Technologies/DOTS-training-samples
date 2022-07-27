using Unity.Entities;
using Unity.Mathematics;

public struct Bee : IComponentData
{
    public Entity target;

    public float3 targetPos;
}
