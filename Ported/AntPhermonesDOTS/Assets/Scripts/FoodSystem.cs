using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class FoodSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem EntityCommandBufferSystem;
    
    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<Map>();
        RequireSingletonForUpdate<FoodSpawner>();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps.Complete();
        
        var RNG = new Random(3);
        var commandBuffer = EntityCommandBufferSystem.CreateCommandBuffer();
        Map map = GetSingleton<Map>();
        Entity entity = GetSingletonEntity<FoodSpawner>();
        FoodSpawner foodSpawner = GetSingleton<FoodSpawner>();
        int2 offset = RNG.NextInt2(0,map.Size);
        
        // Place the food prefab at random location.
        var instance = commandBuffer.Instantiate(foodSpawner.Prefab);
        commandBuffer.SetComponent(instance, new Translation {Value = new float3(offset.x, offset.y, 0.0f)});
        commandBuffer.DestroyEntity(entity);

        return default;
    }
}