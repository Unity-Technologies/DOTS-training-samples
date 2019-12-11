using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class SpawnSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var jobHandle = Entities
            .WithName("SpawnerSystem")
            .ForEach((Entity entity, int entityInQueryIndex, ref Spawner spawner) =>
            {
                spawner.Dummy += 1;
                if (spawner.Dummy % 25 == 0)
                {
                    var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.Prefab);
                    var position = new float3((float)spawner.Dummy * 0.2f,0,0);
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation {Value = position});
                }

            }).Schedule(inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
