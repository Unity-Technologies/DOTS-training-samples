using Unity.Entities;
using Unity.Mathematics;
public struct Turret : IComponentData
{
    public float3 position;
    public float3 rotation;
    public Entity cannonBall;

}
