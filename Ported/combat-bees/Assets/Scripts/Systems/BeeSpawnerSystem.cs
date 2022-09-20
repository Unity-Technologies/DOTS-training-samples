using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
    
[BurstCompile]
partial struct BeeSpawnerSystem : ISystem
{
    private EntityQuery BeeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Todo : Change all this, for proper behavior.
        
        var config = SystemAPI.GetSingleton<BeeConfig>();
        
        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        var random = Random.CreateFromIndex(1234);
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        float radius = 10.0f;
        
        var beeArray = CollectionHelper.CreateNativeArray<Entity>(config.beeCount, Allocator.Temp);
        ecb.Instantiate(config.bee, beeArray);

        foreach (var bee in beeArray)
        {
            var pos = random.NextFloat3();
            pos *= radius;
            var tm = state.EntityManager.GetComponentData<LocalToWorldTransform>(config.food);
            tm.Value.Position = pos;
            ecb.SetComponent<LocalToWorldTransform>(bee, tm);
        }
        
        // force disable system after the first update call
        state.Enabled = false;
    }
}