using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;

public class BoardInitSystem : SystemBase
{
    private KeyboardInput m_KeyboardInput;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate()
    {
        m_KeyboardInput = Object.FindObjectsOfType<KeyboardInput>().FirstOrDefault();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {

        if (m_KeyboardInput.ResetScenePending)
        {
            {
                // This is all a bit pointless?

                var ecb = new EntityCommandBuffer(Allocator.TempJob);

                Entities.WithAll<WallTag>().WithName("DestroyWalls").ForEach((Entity entity) =>
                {
                    ecb.DestroyEntity(entity);
                }).Schedule();

                Entities.WithAll<Arc>().WithName("DestroyArcs").ForEach((Entity entity) =>
                {
                    ecb.DestroyEntity(entity);
                }).Schedule();

                Dependency.Complete();
                ecb.Playback(EntityManager);
                ecb.Dispose();
            }

            Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 10000));

            Entities.WithName("ResetAnts").ForEach((ref AntTag ant, ref Translation translation, ref Yaw yaw) =>
            {
                translation.Value = float3.zero;
                ant.HasFood = false;
                ant.GoalSeekAmount = 0.0f;

                yaw.CurrentYaw = random.NextFloat(-math.PI, math.PI);
            }).ScheduleParallel();

            var ecb2 = new EntityCommandBuffer(Allocator.TempJob);
            var jobHandle2 = Entities.WithAll<FoodTag>().WithName("AddFoodSpawn").ForEach((Entity entity) =>
            {
                ecb2.AddComponent<FoodSpawnAuthoring>(entity);
            }).Schedule(Dependency);

            // Flag jobs so they can start to be run
            JobHandle.ScheduleBatchedJobs();

            // Reset pheromones
            //             {
            //                 UnityEngine.Profiling.Profiler.BeginSample("ResetPheromones");
            //                 var mapEntity = GetSingletonEntity<PheromoneMap>();
            //                 var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
            //                 var pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity);
            //                 for (int i = 0; i < pheromones.Length; i++)
            //                 {
            //                     pheromones[i] = 0.0f;
            //                 }
            // 
            //                 UnityEngine.Profiling.Profiler.EndSample();
            //             }
            Entities
                .WithName("ResetPheromones")
                .ForEach((ref DynamicBuffer<PheromoneStrength> pheromones, in PheromoneMap map) =>
            {
                for (int i = 0; i < pheromones.Length; i++)
                {
                    pheromones[i] = 0.0f;
                }
            }).Schedule();

            m_KeyboardInput.ResetScenePending = false;

            var combined = JobHandle.CombineDependencies(jobHandle2, Dependency);
            combined.Complete();
            ecb2.Playback(EntityManager);

            ecb2.Dispose();
        }

        Entities.WithStructuralChanges().ForEach((Entity entity, in BoardInitTag boardInitTag) =>
        {
            var boardInit = GetSingleton<BoardInitAuthoring>();
            for (int i = 1; i <= boardInit.NumberOfRings; i++)
            {
                //have a random seed
                Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 10000));
                float startAngle = random.NextFloat(0, 359);
                float endAngle = random.NextFloat(startAngle + boardInit.MinRingWidth, startAngle + boardInit.MaxRingWidth);

                Entity arcEntity = CreateArcEntity(boardInit, random, startAngle, endAngle, i * boardInit.SpaceBetweenTheRings);
                Arc arc = EntityManager.GetComponentData<Arc>(arcEntity);

                float diff = math.abs(arc.EndAngle - arc.StartAngle);
                if (diff <= boardInit.DualRingThreshold)
                {
                    float opening = (360 - (diff * 2)) / 2;
                    float start = arc.EndAngle + opening;
                    float end = start + diff;
                    CreateArcEntity(boardInit, random, startAngle, endAngle, i * boardInit.SpaceBetweenTheRings);
                }
            }

            EntityManager.RemoveComponent<BoardInitTag>(entity);
        }).Run();


        //Place Food
        Entities.WithStructuralChanges().WithAll<FoodTag>().ForEach((Entity entity, in LocalToWorld ltw, in FoodSpawnAuthoring foodSpawn) =>
        {
          // return;

          Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 10000));
            float foodAngleRad = random.NextFloat(0, math.radians(360.0f));
            float foodRadius = 20.0f;

            float3 position = new float3(math.sin(foodAngleRad) * foodRadius, 0, math.cos(foodAngleRad) * foodRadius);

            SetComponent(entity, new Translation { Value = position });

            EntityManager.RemoveComponent<FoodSpawnAuthoring>(entity);

        }).Run();

    }

    Entity CreateArcEntity(BoardInitAuthoring boardInit, Random random, float startAngle, float endAngle, float radius)
    {
        //add and split
        EntityArchetype archetype = EntityManager.CreateArchetype(
            typeof(Arc)
        );

        //add new arc entity
        Entity arc = EntityManager.CreateEntity(archetype);
        EntityManager.AddComponentData(arc, new Arc
        {
            Radius = radius,
            StartAngle = startAngle,
            EndAngle = random.NextFloat(startAngle + boardInit.MinRingWidth, startAngle + boardInit.MaxRingWidth),
        });
        
        //create arcs
        for (int i = (int)startAngle; i < (endAngle + 1); i++)
        {
            float rad = math.radians(i);
            float3 position = new float3(math.sin(rad) * radius, 0, (math.cos(rad) * radius));

            //instantiate prefabs with mesh render
            var instance = EntityManager.Instantiate(boardInit.wallPrefab);
            SetComponent(instance, new Translation { Value = position });
        }

        return arc;
    }
    
    static float ClampAngle(float angle)
    {
        float result = angle - math.ceil((angle / 360f) * 360f);
        if (result < 0)
        {
            result += 360f;
        }
        return result;

    }



}
