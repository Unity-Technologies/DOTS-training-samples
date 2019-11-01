using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;
using UnityEngine;

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
        //[ReadOnly] public float3 pos;
        [ReadOnly] public Random random;
        public void Execute(Entity entity, int entityIndex, [ReadOnly] ref BarSpawner spawnerFromEntity, 
            [ReadOnly] ref Translation translation)
        {

            float spacing = 2f;
            for (var x = 0; x < spawnerFromEntity.buildingCount; x++)
            {
                float3 pos = new float3(random.NextFloat(-45f, 45f), 0f, random.NextFloat(-45f, 45f));
                float height = random.NextInt(4, 12);
                for (int i = 0; i < height; i++)
                {
                    BarPoint point1 = new BarPoint { index = 1 };
                    point1.pos.x = pos.x + spacing;
                    point1.pos.y = i * spacing;
                    point1.pos.z = pos.z - spacing;
                    point1.oldPos.x = point1.pos.x;
                    point1.oldPos.y = point1.pos.y;
                    point1.oldPos.z = point1.pos.z;

                    BarPoint point2 = new BarPoint { index = 2 };
                    point2.pos.x = pos.x - spacing;
                    point2.pos.y = i * spacing;
                    point2.pos.z = pos.z - spacing;
                    point2.oldPos.x = point2.pos.x;
                    point2.oldPos.y = point2.pos.y;
                    point2.oldPos.z = point2.pos.z;

                    BarPoint point3 = new BarPoint { index = 3 };
                    point3.pos.x = pos.x + spacing;
                    point3.pos.y = i * spacing;
                    point3.pos.z = pos.z + spacing;
                    point3.oldPos.x = point3.pos.x;
                    point3.oldPos.y = point3.pos.y;
                    point3.oldPos.z = point3.pos.z;

                    var point1Entity = CommandBuffer.CreateEntity(entityIndex);
                    CommandBuffer.AddComponent(entityIndex, point1Entity, point1);
                    var point2Entity = CommandBuffer.CreateEntity(entityIndex);
                    CommandBuffer.AddComponent(entityIndex, point2Entity, point2);
                    var point3Entity = CommandBuffer.CreateEntity(entityIndex);
                    CommandBuffer.AddComponent(entityIndex, point3Entity, point3);

                    float3 delta = point2.pos - point1.pos;
                    float3 newPos = new float3(point1.pos.x + point2.pos.x, point1.pos.y + point2.pos.y, point1.pos.z + point2.pos.z) * .5f;
                    Bar bar1 = new Bar { point1 = point1Entity, point2 = point2Entity, length = math.length(delta), thickness = random.NextFloat(.25f,.35f) };
                    var barentity1 = CommandBuffer.Instantiate(entityIndex, spawnerFromEntity.particlePrefab);
                    CommandBuffer.AddComponent(entityIndex, barentity1, bar1);
                    CommandBuffer.SetComponent(entityIndex, barentity1, new Translation { Value = newPos });
                    CommandBuffer.SetComponent(entityIndex, barentity1, new Rotation { Value = quaternion.LookRotation(math.normalize(delta), new float3(0, 1, 0)) });

                    delta = point3.pos - point2.pos;
                    newPos = new float3(point2.pos.x + point3.pos.x, point2.pos.y + point3.pos.y, point2.pos.z + point3.pos.z) * .5f;
                    Bar bar2 = new Bar { point1 = point2Entity, point2 = point3Entity, length = math.length(delta), thickness = random.NextFloat(.25f,.35f) };
                    var barentity2 = CommandBuffer.Instantiate(entityIndex, spawnerFromEntity.particlePrefab);
                    CommandBuffer.AddComponent(entityIndex, barentity2, bar2);
                    CommandBuffer.SetComponent(entityIndex, barentity2, new Translation { Value = newPos });
                    CommandBuffer.SetComponent(entityIndex, barentity2, new Rotation { Value = quaternion.LookRotation(math.normalize(delta), new float3(0, 1, 0)) });

                    delta = point1.pos - point3.pos;
                    newPos = new float3(point3.pos.x + point1.pos.x, point3.pos.y + point1.pos.y, point3.pos.z + point1.pos.z) * .5f;
                    Bar bar3 = new Bar { point1 = point3Entity, point2 = point1Entity, length = math.length(delta), thickness = random.NextFloat(.25f,.35f) };
                    var barentity3 = CommandBuffer.Instantiate(entityIndex, spawnerFromEntity.particlePrefab);
                    CommandBuffer.AddComponent(entityIndex, barentity3, bar3);
                    CommandBuffer.SetComponent(entityIndex, barentity3, new Translation { Value = newPos });
                    CommandBuffer.SetComponent(entityIndex, barentity3, new Rotation { Value = quaternion.LookRotation(math.normalize(delta), new float3(0, 1, 0)) });

                }
            }

            CommandBuffer.DestroyEntity(entityIndex, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var random = new Random(5);
        
        // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
        var job = new BuildingSpawnSystemJob
        {
            
            //pos = new float3(UnityEngine.Random.Range(-45f, 45f), 0f, UnityEngine.Random.Range(-45f, 45f)),
            random = random,
        CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDeps);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}