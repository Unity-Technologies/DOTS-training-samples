using Unity.Entities;

public struct BeeConfig : IComponentData
{
    public Entity bee; 
    public Entity food;
    public int beeCount;
    public int foodCount;
}
