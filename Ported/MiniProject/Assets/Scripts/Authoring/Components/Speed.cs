using Unity.Entities;
using Unity.Mathematics;

struct Speed : IComponentData
{
    public float3 speed;
    public float dragFactor;
    public float bounceFactor;
}
