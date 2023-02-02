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
    private ComponentLookup<Stair> m_StairsLookupRO;
    private BufferLookup<PlatformStairs> m_PlatformStairsLookupRO;



    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();
        m_StairsLookupRO = state.GetComponentLookup<Stair>(true);
        m_PlatformStairsLookupRO = state.GetBufferLookup<PlatformStairs>(true);

    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        m_StairsLookupRO.Update(ref state);
        m_PlatformStairsLookupRO.Update(ref state);

        var config = SystemAPI.GetSingleton<Config>();
        var random = Unity.Mathematics.Random.CreateFromIndex(1234);

        NativeArray<Entity> stations = new NativeArray<Entity>();
        foreach (var (station, stationEntity) in SystemAPI.Query<Station>().WithEntityAccess())
        {
            //Disabling steps color change as the original implementations depended on the Stairs entity in the Platform component, which doesn't exist now (using buffers instead)
            //SetStepsColor(state, station, stationEntity, m_StairsLookupRO, m_PlatformStairsLookupRO);

        }
        state.Enabled = false;
    }

    void SetStepsColor(SystemState state, Station station, Entity stationEntity, ComponentLookup<Stair> stairsLookupRO, BufferLookup<PlatformStairs> platformStairsLookupRO)
    {
        var random = Unity.Mathematics.Random.CreateFromIndex(1234);
        var hue = random.NextFloat();
        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        int colorIndex = 0;
        foreach (var platfomChild in /*station.Platforms*/ SystemAPI.GetBuffer<Child>(stationEntity))
        {
            var platfom = state.EntityManager.GetComponentData<Platform>(platfomChild.Value);

            platformStairsLookupRO.TryGetBuffer(platfomChild.Value, out var currentPlatformStairsBuffer);
            //int platformStairsIndex = 0;
            for (int i = 0; i < currentPlatformStairsBuffer.Length; ++i)
            {
                stairsLookupRO.TryGetComponent(currentPlatformStairsBuffer[i].Stairs, out var stairs);

                ////////////////
                //if (SystemAPI.HasBuffer<Child>(currentPlatformStairsBuffer[i].Stairs))
                //{
                //    foreach (var step in SystemAPI.GetBuffer<Child>(currentPlatformStairsBuffer[i].Stairs))
                //    {
                //        if (state.EntityManager.HasComponent<URPMaterialPropertyBaseColor>(step.Value))
                //        {
                //            state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>
                //                (step.Value, RandomColor());
                //                //new URPMaterialPropertyBaseColor
                //                //{
                //                //    Value = /*station.Colors[colorIndex]*/RandomColor()
                //                //});
                //        }
                //    }
                //}
            }
        }
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




