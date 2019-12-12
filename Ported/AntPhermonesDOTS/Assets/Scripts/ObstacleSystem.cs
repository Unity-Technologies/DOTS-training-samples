using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct ObstacleInitializedTag : IComponentData
{
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ObstacleSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        Map map = GetSingleton<Map>();
        var RNG = new Random(1); // TODO not deterministic across threads?
        
        // Place the instantiated prefabs in pre-calculated concentric circles with holes.
        var jobHandle = Entities
            .WithName("ObstacleSystem")
            .WithNone<ObstacleInitializedTag>()
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, in Obstacle obstacle) =>
            {
                var posArray = obstacle.Blob.Value.Positions.ToArray();
                foreach (var position in posArray)
                {
                    var instance = commandBuffer.Instantiate(entityInQueryIndex, obstacle.Prefab);
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation {Value = position});
                }
                commandBuffer.AddComponent<ObstacleInitializedTag>(entityInQueryIndex, entity);
            }).Schedule(inputDeps);

        EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}