using Unity.Entities;
using Unity.Mathematics;

struct Rock : IComponentData
{
    public float3 size;
    public float initialHealth;
}
