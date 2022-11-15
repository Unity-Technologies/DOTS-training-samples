using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct PathPlanning : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();
        
        var grid = state.EntityManager.GetBuffer<GridCell>(gameConfigEntity);
        
        
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}
