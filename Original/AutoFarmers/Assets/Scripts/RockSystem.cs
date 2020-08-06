using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;



public class RockSystem : SystemBase
{
    private EntityQuery m_RocksQuery;
    private EntityCommandBufferSystem m_ECBSystem;

    

    protected override void OnCreate()
    {
        m_RocksQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Rock>()
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        Entities
        .WithStructuralChanges()
        .ForEach((Entity entity, in RockDataSpawner rockDataSpawner) =>
        {
            SpawnRock(rockDataSpawner.rockCount, rockDataSpawner);
            EntityManager.RemoveComponent<RockDataSpawner>(entity);

        }).Run();

        
        var rocksRock = m_RocksQuery.ToComponentDataArrayAsync<Rock>(Allocator.TempJob, out var rocksRockHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, rocksRockHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        // TODO: Hmmm, not really that good...
        /*
        Entities
            .WithDisposeOnCompletion(rocksRock)
            .WithAll<Rock>()
            .ForEach((int entityInQueryIndex, Entity entity, in Rock rock) =>
            {
                //Debug.Log(rock.rectInt);
                for (int i = entityInQueryIndex + 1; i < rocksRock.Length; ++i)
                {
                    if (rock.rectInt.Overlaps(rocksRock[i].rectInt, true))
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }
            }).ScheduleParallel();
        */

        // Destroy rock if health is <= 0
        //var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .ForEach((int entityInQueryIndex, Entity rockEntity, ref Health health) =>
            {
                var currentHealth = health.Value;
                if(currentHealth <= 0)
                {
                    ecb.DestroyEntity(entityInQueryIndex, rockEntity);
                }
                // health.Value = currentHealth -= 1; // Testing only

            }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);

    }

    internal void SpawnRock(int count, in RockDataSpawner rockData)
    {

        for (int i = 0; i < count; ++i)
        {
            var rock = EntityManager.Instantiate(rockData.prefab);

            EntityManager.AddComponent<NonUniformScale>(rock);
            EntityManager.AddComponent<RockRect>(rock);
            EntityManager.AddComponent<Health>(rock);
            EntityManager.AddComponent<BatchNumber>(rock);

            // Init
            int width = UnityEngine.Random.Range(0, 4);
            int height = UnityEngine.Random.Range(0, 4);
            int rockX = UnityEngine.Random.Range(0, rockData.mapSize.x - width);
            int rockY = UnityEngine.Random.Range(0, rockData.mapSize.y - height);
            Rect rect = new Rect(rockX, rockY, width, height);
            
            float health = (rect.width + 1.0f) * (rect.height + 1.0f) * 15;
            float startHealth = health;
            float depth = UnityEngine.Random.Range(.4f, .8f);

            Vector2 center2D = rect.center;
            Vector3 worldPos = new Vector3(center2D.x + .5f, depth * .5f, center2D.y + .5f);
            Vector3 scale = new Vector3(rect.width + .5f, depth, rect.height + .5f);
            Matrix4x4 matx = Matrix4x4.TRS(worldPos, Quaternion.identity, scale);

            EntityManager.AddComponentData<Rock>(rock, new Rock { matrix = matx, rectInt = rect });
            EntityManager.AddComponentData<Health>(rock, new Health { Value = health });
            EntityManager.AddComponentData<StartHealth>(rock, new StartHealth { Value = health });

            EntityManager.SetComponentData<Translation>(rock, new Translation { Value = new Unity.Mathematics.float3(center2D.x + .5f, depth * .5f, center2D.y + .5f) });
            EntityManager.SetComponentData<NonUniformScale>(rock, new NonUniformScale { Value = new Unity.Mathematics.float3(rect.width + .5f, depth, rect.height + .5f) });
            
        }
    }

    

}
