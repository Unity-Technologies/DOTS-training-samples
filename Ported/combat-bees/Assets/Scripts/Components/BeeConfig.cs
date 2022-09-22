using Unity.Entities;
using Unity.Mathematics;

public struct BeeConfig : IComponentData
{
    public Entity bee; 
    public Entity food;
    public int beeCount;
    public int foodCount;
    public AABB fieldArea;
    public float3 initVel;
    public float beeSpeed;
    public float gravity;
    public float objectSize;
}
