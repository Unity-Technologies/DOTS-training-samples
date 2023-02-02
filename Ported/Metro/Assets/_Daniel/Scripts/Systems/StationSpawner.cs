using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(LineSpawner))]
[BurstCompile]
public partial struct StationSpawner : ISystem
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

        NativeArray<Entity> stations = new NativeArray<Entity>();
        foreach (var (line, lineEntity) in SystemAPI.Query<Line>().WithEntityAccess())
        {
            stations = SpawnStations(state, lineEntity, line, config);
        }
        state.Enabled = false;
    }

    private NativeArray<Entity> SpawnStations(SystemState state, Entity lineEntity, Line line, Config config)
    {
        var stations = state.EntityManager.Instantiate(config.StationPrefab, config.StationsPerLineCount /*config.LineCount*/, Allocator.Temp);
        float stationCounter = 0f;
        float stationOffset = config.StationsOffset;
        float lineOffset = config.LineOffset;

        foreach (var station in stations)
        {
            var stationTransform = LocalTransform.FromPosition(new float3(0, 0, line.Id * lineOffset) + new float3(stationOffset * stationCounter, 0, 0));
            state.EntityManager.SetComponentData<LocalTransform>(station, stationTransform);
            //var stationData = state.EntityManager.GetComponentData<Station>(station);
            //Need to set proper colors
            //state.EntityManager.SetComponentData<Station>(station, stationData);
            stationCounter++;
        }
        return stations;
    }
}

