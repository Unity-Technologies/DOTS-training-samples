using Unity.Burst;
using Unity.Entities;

[BurstCompile]
partial struct FireSpreadingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Look for entities that have a tile component with heat higher than 0.
    }
}