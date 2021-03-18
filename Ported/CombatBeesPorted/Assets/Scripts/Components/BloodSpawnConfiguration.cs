using Unity.Entities;
using Unity.Mathematics;

struct BloodSpawnConfiguration : IComponentData
{
    public float3 Direction;
    public int Count;
}