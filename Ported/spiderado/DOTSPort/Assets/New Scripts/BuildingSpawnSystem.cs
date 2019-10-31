using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class BuildingSpawnSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct BuildingSpawnSystemJob : IJobForEachWithEntity<BarSpawner, Translation>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int entityIndex, [ReadOnly] ref BarSpawner spawnerFromEntity, 
            [ReadOnly] ref Translation translation)
        {
            for (var x = 0; x < spawnerFromEntity.buildingCount; x++)
            {
                BarPoint point1 = new BarPoint { index = 1 };
                BarPoint point2 = new BarPoint { index = 2 };
                BarPoint point3 = new BarPoint { index = 3 };
                var point1Entity = CommandBuffer.CreateEntity(entityIndex);
                CommandBuffer.AddComponent(entityIndex, point1Entity, point1);
                var point2Entity = CommandBuffer.CreateEntity(entityIndex);
                CommandBuffer.AddComponent(entityIndex, point2Entity, point2);
                var point3Entity = CommandBuffer.CreateEntity(entityIndex);
                CommandBuffer.AddComponent(entityIndex, point3Entity, point3);

                Bar bar1 = new Bar { point1 = point1Entity, point2 = point2Entity };
                var barentity1 = CommandBuffer.Instantiate(entityIndex, spawnerFromEntity.particlePrefab);
                CommandBuffer.AddComponent(entityIndex, barentity1, bar1);
                Bar bar2 = new Bar { point1 = point2Entity, point2 = point3Entity };
                var barentity2 = CommandBuffer.Instantiate(entityIndex, spawnerFromEntity.particlePrefab);
                CommandBuffer.AddComponent(entityIndex, barentity2, bar2);
                Bar bar3 = new Bar { point1 = point3Entity, point2 = point1Entity };
                var barentity3 = CommandBuffer.Instantiate(entityIndex, spawnerFromEntity.particlePrefab);
                CommandBuffer.AddComponent(entityIndex, barentity3, bar3);


                // Place the instantiated in a grid with some noise
                var position = new float3(x * 1.3F, noise.cnoise(new float2(x, 2) * 0.21F) * 50, 2 * 1.3F);
                CommandBuffer.SetComponent(entityIndex, barentity1, new Translation { Value = position });
                CommandBuffer.SetComponent(entityIndex, barentity2, new Translation { Value = position + new float3(1f) });
                CommandBuffer.SetComponent(entityIndex, barentity3, new Translation { Value = position + new float3(2f) });  
                                 
            }

            CommandBuffer.DestroyEntity(entityIndex, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
        var job = new BuildingSpawnSystemJob
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDeps);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}