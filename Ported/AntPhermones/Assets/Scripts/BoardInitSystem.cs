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

                Entities.WithAll<DynamicArcTag>().WithName("DestroyDynamicArcs").ForEach((Entity entity) =>
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

            var ecb1 = new EntityCommandBuffer(Allocator.TempJob);
            var jobHandle1 = Entities.WithAll<Arc>().WithName("AddWallAuthoring").ForEach((Entity entity) =>
            {
                ecb1.AddComponent<WallAuthoring>(entity);
            }).Schedule(Dependency);

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

            var combined = JobHandle.CombineDependencies(jobHandle1, jobHandle2, Dependency);
            combined.Complete();
            ecb1.Playback(EntityManager);
            ecb2.Playback(EntityManager);

            ecb1.Dispose();
            ecb2.Dispose();
        }

        //There should be at least 3 entities
        Entities.WithStructuralChanges().ForEach((Entity entity,
            ref Arc arc, in WallAuthoring wall, in LocalToWorld ltw) =>
        {
            float deg2rad = (math.PI * 2) / 360;
            float minRingWidth = 120; //temp
          float maxRingWidth = 300; //temp

          var wallSettingsEntity = GetSingletonEntity<WallSettings>();
            var wallSettings = EntityManager.GetComponentData<WallSettings>(wallSettingsEntity);

          //have a random seed
          Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 10000));

          //the split arcs' angles will be set upon creatin
          if (arc.split == 0)
            {
                arc.StartAngle = random.NextFloat(0, 359);
                arc.EndAngle = random.NextFloat(arc.StartAngle + minRingWidth, arc.StartAngle + maxRingWidth);
            }

            float diff = math.abs(arc.EndAngle - arc.StartAngle);
            if (arc.split == 0 && diff <= 165)
            {
                float opening = (360 - (diff * 2)) / 2;
                float start = arc.EndAngle + opening;
                float end = start + diff;

              //add and split
              EntityArchetype archetype = EntityManager.CreateArchetype(
                 typeof(Arc),
                 typeof(WallAuthoring),
                 typeof(LocalToWorld),
                 typeof(DynamicArcTag)
                 );

              //add new arc entity
              Entity splitArc = EntityManager.CreateEntity(archetype);
                EntityManager.AddComponentData(splitArc, new WallAuthoring());
                EntityManager.AddComponentData(splitArc, new Arc
                {
                    Radius = arc.Radius,
                    StartAngle = start,
                    EndAngle = end,
                    split = 1
                });
                EntityManager.AddComponentData(splitArc, new DynamicArcTag());
            }

          //create arcs
          for (int i = (int)arc.StartAngle; i < (arc.EndAngle + 1); i++)
            {
                float rad = deg2rad * i;
                float3 position = new float3(ltw.Position.x + (math.sin(rad) * arc.Radius), 0,
                 ltw.Position.z + (math.cos(rad) * arc.Radius));

              //instantiate prefabs with mesh render
              var instance = EntityManager.Instantiate(wallSettings.wallPrefab);
                SetComponent(instance, new Translation { Value = position });
            }

          //Only run this once
          EntityManager.RemoveComponent<WallAuthoring>(entity);

        }).WithoutBurst().Run();


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
