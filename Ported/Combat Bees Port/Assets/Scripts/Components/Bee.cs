using Unity.Entities;
using Unity.Mathematics;

public struct Bee : IComponentData
{
    public float3 position;
    public float3 rotation;

    public Entity target;

    public BeeState state;
}
