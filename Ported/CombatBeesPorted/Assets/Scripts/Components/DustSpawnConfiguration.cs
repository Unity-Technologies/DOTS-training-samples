using Unity.Entities;
using Unity.Mathematics;

struct DustSpawnConfiguration : IComponentData
{
    public float3 Direction;
    public int Count;
}