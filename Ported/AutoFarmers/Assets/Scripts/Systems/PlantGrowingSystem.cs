using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct PlantGrowingSystem : ISystem
{
    public void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<GameConfig>();
    }

    public void OnDestroy(ref SystemState state) 
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var growthRate = 1f / SystemAPI.GetSingleton<GameConfig>().PlantIncubationTime;
        var dt = state.Time.DeltaTime;

        var growth = dt * growthRate;
        foreach (var plant in SystemAPI.Query<PlantGrowingAspect>().WithAll<PlantGrowing>())
        {
            plant.Health += growth;
            if (plant.GrowingComplete)
            {
                ecb.RemoveComponent<PlantGrowing>(plant.Self);
                ecb.AddComponent(plant.Self, new PlantGrown { });
            }
        }
    }
}
