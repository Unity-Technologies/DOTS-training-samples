using System.Xml;
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
                var rng = new Random(123);
                
                var count = 6;
                for (var x = 0; x < count; ++x)
                {
                    for (var z = 0; z < count; ++z)
                    {
                        var instance = commandBuffer.Instantiate(entityInQueryIndex, spawner.Prefab);
                        //var position = new float3(x*1.25f,z*1.25f,0) - new float3(64, 64, 0);
                        var position = float3.zero;
                        commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation {Value = position});
                        commandBuffer.AddComponent<AntTag>(entityInQueryIndex, instance);
                        commandBuffer.AddComponent(entityInQueryIndex, instance, new Velocity { Speed = 0.0f, Rotation = rng.NextFloat(2.0f * math.PI) });
                    }
                }
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);

            }).Schedule(inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
