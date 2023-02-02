using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(StationSpawner))]
[BurstCompile]
public partial struct PlatformSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;
    ComponentLookup<WorldTransform> m_WorldTransformLookup;

    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var random = Unity.Mathematics.Random.CreateFromIndex(1234);

        NativeArray<Entity> stations = new NativeArray<Entity>();
        foreach (var (station, stationEntity) in SystemAPI.Query<Station>().WithEntityAccess())
        {
            //stations = SpawnStations(state, lineEntity, line, config);
           SetStepsColor(state, station, stationEntity);

        }
        state.Enabled = false;
    }

    void SetStepsColor(SystemState state, Station station, Entity stationEntity)
    {
        int colorIndex = 0;
            foreach (var platfomChild in /*station.Platforms*/ SystemAPI.GetBuffer<Child>(stationEntity))
            {
                var platfom = state.EntityManager.GetComponentData<Platform>(platfomChild.Value);
                if (SystemAPI.HasBuffer<Child>(platfom.Stairs))
                {
                    foreach (var step in SystemAPI.GetBuffer<Child>(platfom.Stairs))
                    {
                        if (state.EntityManager.HasComponent<URPMaterialPropertyBaseColor>(step.Value))
                        {
                        state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>
                            (step.Value,
                            new URPMaterialPropertyBaseColor
                            {
                                Value = station.Colors[colorIndex]
                            }) ;
                        }
                    }
                }
            }
            colorIndex++;
        }
    }
    /*   private void SpawnPlatforms(SystemState state, Station station, Entity s, Entity e, Config config, ComponentLookup<WorldTransform> wt)
       {
           int platformCount = 4 ;
           var platforms = state.EntityManager.Instantiate(config.PlatformPrefab, platformCount, Allocator.Temp);
           float lineCounter = 0f;
           float offset = 30f;

           //var spawnLocalToWorld = wt[e];
           //var platformTransform = LocalTransform.FromPosition(spawnLocalToWorld.Position + new float3(lineCounter));

           foreach (var platform in platforms)
           {
               var platformTransform = LocalTransform.FromPosition(new float3(offset * lineCounter, 0, 0));

               var position = new float3(SystemAPI.Time.ElapsedTime * 10);
               position.xz += (lineCounter);
               //URPMaterialPropertyBaseColor color = RandomColor();

               state.EntityManager.SetComponentData<LocalTransform>(platform, platformTransform);

               //SetStairColor(state, station);

               lineCounter++;
           }
           //station.Platforms = platforms;
     }
   */




