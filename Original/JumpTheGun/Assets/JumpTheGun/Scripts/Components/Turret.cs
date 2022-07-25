using Unity.Entities;
using Unity.Mathematics;
public struct Turret : IComponentData
{
    public float3 position;
    public float3 rotation;
    public float3 scale;
    public float4 color;

    public Entity cannonBall;

}
