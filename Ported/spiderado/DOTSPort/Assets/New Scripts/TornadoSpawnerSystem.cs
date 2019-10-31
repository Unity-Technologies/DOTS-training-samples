using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

// ReSharper disable once InconsistentNaming
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TornadoSpawnerSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    struct TornadoSpawnJob : IJobForEachWithEntity<Spawner, LocalToWorld>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref Spawner spawnerFromEntity,
            [ReadOnly] ref LocalToWorld location)
        {
            for (var x = 0; x < spawnerFromEntity.gridX; x++)
            {
                for (var y = 0; y < spawnerFromEntity.gridY; y++)
                {
                    var instance = CommandBuffer.Instantiate(index, spawnerFromEntity.particlePrefab);

                    // Place the instantiated in a grid with some noise
                    var position = new float3(x * 1.3F, noise.cnoise(new float2(x, y) * 0.21F) * 50, y * 1.3F);
                    CommandBuffer.SetComponent(index, instance, new Translation {Value = position});
                    CommandBuffer.AddComponent(index, instance, new TornadoFlagComponent { isVortex = true });
                    
                    
                }                    
            }

            CommandBuffer.DestroyEntity(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
        var job = new TornadoSpawnJob
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDeps);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}
