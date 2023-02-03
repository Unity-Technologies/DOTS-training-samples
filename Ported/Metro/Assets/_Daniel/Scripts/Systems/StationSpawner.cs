using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(LineSpawner))]
[BurstCompile]
public partial struct StationSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;
    ComponentLookup<WorldTransform> m_WorldTransformLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();
        m_WorldTransformLookup = state.GetComponentLookup<WorldTransform>(true);

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_WorldTransformLookup.Update(ref state);

        var config = SystemAPI.GetSingleton<Config>();

        int lineSetCount = config.LineCount;

        for (int i = 0; i < lineSetCount; i++)
            SpawnStations(state, i, config);

        state.Enabled = false;
    }

    private void SpawnStations(SystemState state, int systemId, Config config)
    {
        foreach (var (line, lineEntity) in SystemAPI.Query<Line>().WithEntityAccess())
        {
            if(line.SystemId == systemId)
            {
                var stations = state.EntityManager.Instantiate(config.StationPrefab, config.StationsPerLineCount, Allocator.Temp);
                int stationCounter = 0;
                float stationOffset = config.StationsOffset;
                float lineOffset = config.LineOffset;

                foreach (var station in stations)
                {
                    var stationTransform = LocalTransform.FromPosition(new float3(0, 0, systemId * lineOffset) + new float3(stationOffset * stationCounter, 0, 0));
                    state.EntityManager.SetComponentData<LocalTransform>(station, stationTransform);
                    state.EntityManager.SetComponentData<Station>(station, new Station {
                        Line = lineEntity,
                        Id = stationCounter,
                        SystemId = systemId
                    });
                    stationCounter++;
                }
                return;
            }
        }
    } 
}

