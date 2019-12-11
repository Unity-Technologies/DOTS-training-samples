using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ColonySystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem EntityCommandBufferSystem;
    
    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps.Complete();
        
        var commandBuffer = EntityCommandBufferSystem.CreateCommandBuffer();
        //Map map = GetSingleton<Map>();
        Entity entity = GetSingletonEntity<Colony>();
        Colony colony = GetSingleton<Colony>();

        // Place the colony prefab in the center.
        var instance = commandBuffer.Instantiate(colony.Prefab);
        commandBuffer.SetComponent(instance, new Translation {Value = 0.0f});
        commandBuffer.DestroyEntity(entity);

        return default;
    }
}