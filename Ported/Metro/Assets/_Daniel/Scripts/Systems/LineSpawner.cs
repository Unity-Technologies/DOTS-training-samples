using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
partial struct LineSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;
    ComponentLookup<WorldTransform> m_WorldTransformLookup;


    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();
        m_WorldTransformLookup = state.GetComponentLookup<WorldTransform>(true);

    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        m_WorldTransformLookup.Update(ref state);

        var config = SystemAPI.GetSingleton<Config>();
        var random = Unity.Mathematics.Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        NativeArray<Entity> lines = new NativeArray<Entity>(config.LineCount, Allocator.Persistent);
        for (int i = 0; i < config.LineCount; i++)
        {
            Entity e = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<Line>(e);
            state.EntityManager.SetComponentData<Line>(e, new Line { LineColor = RandomColor().Value });
            NativeArray<Entity> stations = SpawnStations(state, e, config, m_WorldTransformLookup);
        }
        state.Enabled = false;
    }

    private NativeArray<Entity> SpawnStations(SystemState state, Entity e, Config config, ComponentLookup<WorldTransform> wt)
    {
        var stations = state.EntityManager.Instantiate(config.StationPrefab, config.LineCount, Allocator.Temp);
        Station station = state.EntityManager.GetComponentData<Station>(stations[0]);
        SpawnPlatforms(state, station, e, config, wt);
        return stations;
    }

    private void SpawnPlatforms(SystemState state, Station station, Entity e, Config config, ComponentLookup<WorldTransform> wt)
    {
        int platformCount = 4 /*station.Platforms.Length*/;
        var platforms = state.EntityManager.Instantiate(config.PlatformPrefab, platformCount, Allocator.Temp);
        float lineCounter = 0f;
        float offset = 20f;

        //var spawnLocalToWorld = wt[e];
        //var platformTransform = LocalTransform.FromPosition(spawnLocalToWorld.Position + new float3(lineCounter));

        foreach (var platform in platforms)
        {
            var platformTransform = LocalTransform.FromPosition(new float3(0) + new float3(0,0,offset * lineCounter));

            var position = new float3(SystemAPI.Time.ElapsedTime * 10);
            //state.EntityManager.SetComponentData<LocalTransform>(platform, platformTransform);
            position.xz += (lineCounter);
            //URPMaterialPropertyBaseColor color = RandomColor();
            //state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(line, color);

            //SetStairColor(state, platform, color);
            
            state.EntityManager.SetComponentData<LocalTransform>(platform, platformTransform);

            //state.EntityManager.SetComponentData<LocalTransform>(platform, new LocalTransform { Position = position, Scale =  });
            //state.EntityManager.SetComponentData<WorldTransform>(platform, new WorldTransform { Scale = 1 });

            SetStairColor(state, platform, e);

            lineCounter++;
        }
        station.Platforms = platforms;

    }

    void SetStairColor(SystemState state, Entity platform, /*URPMaterialPropertyBaseColor color*/ Entity e)
    {
        DynamicBuffer<Child> stairSet = SystemAPI.GetBuffer<Child>(platform);
        //foreach (var item in SystemAPI.Query<Child>().WithAll<Parent>().WithAll<Stair>)
        //foreach (var item in SystemAPI.Query</*Stair>().WithAll<Child>().WithAll<*/URPMaterialPropertyBaseColor>())
        Debug.Log("Child count: " + stairSet.Length);
        foreach (var color in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>>())
        {
            //state.EntityManager.SetComponentData()
            color.ValueRW.Value = state.EntityManager.GetComponentData<Line>(e).LineColor;
        }
        foreach (var item in stairSet)
        {
            //if(item != null)
            //state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(item.Value, state.EntityManager.GetComponentData<URPMaterialPropertyBaseColor>(e));
            //DynamicBuffer<Child> steps = SystemAPI.GetBuffer<Child>(platform);
            //foreach (var step in steps)
            //{
            //    state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(step.Value, color);
            //}
        }



        //DynamicBuffer<Child> stairSet = SystemAPI.GetBuffer<Child>(platform);
        //for (int i = 1; i < stairSet.Length - 1; i++)
        //{
        //    DynamicBuffer<Child> steps = SystemAPI.GetBuffer<Child>(stairSet[i].Value);
        //    foreach (var step in steps)
        //    {
        //        state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(step.Value, color);
        //    }
        //}

        //foreach (Child set in stairSet)
        //{

        //    var baseColor = state.EntityManager.GetComponentData<URPMaterialPropertyBaseColor>(set.Value);
        //    //if(baseColor)
        //    {
        //        baseColor = color;
        //        state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(set.Value, baseColor);
        //    }

        //}
    }
}
