using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct WallSpawner : IComponentData
{
    public Entity WallComponent;
    public int RingCount;
}