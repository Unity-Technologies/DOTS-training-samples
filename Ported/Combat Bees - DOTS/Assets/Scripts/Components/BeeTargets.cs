using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public struct BeeTargets : IComponentData
{
    public Entity ResourceTarget;
    public float3 HomePosition;
    public Random Random;
}