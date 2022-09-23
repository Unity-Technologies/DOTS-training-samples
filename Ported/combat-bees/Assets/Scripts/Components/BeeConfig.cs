using Unity.Entities;
using Unity.Mathematics;

public struct BeeConfig : IComponentData
{
    public Entity bee;
    public Entity food;
    public int beeCount;
    public int foodCount;
    public float aggressivity;
    public AABB fieldArea;
    public float3 initVel;
    public float beeSpeed;
    public float gravity;
    public float objectSize;
    public int FoodBeeSpawnCount;
}
