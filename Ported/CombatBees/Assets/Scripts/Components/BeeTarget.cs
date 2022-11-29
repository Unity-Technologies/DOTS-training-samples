using Unity.Entities;
using Unity.Mathematics;

struct BeeTarget : IComponentData
{
    public Entity target;
    public float3 targetPosition;
}