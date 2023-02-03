using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(PlatformSpawner))]
[BurstCompile]
public partial struct TrainSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;
    ComponentLookup<WorldTransform> m_WorldTransformLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
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

        for (int i = 0; i < config.LineCount; i++)
        {
            foreach (var (station, stationEntity) in SystemAPI.Query<Station>().WithEntityAccess())
            {
                if (station.Id == 0 && station.SystemId == i)
                    SpawnTrain(ref state, stationEntity, station, config, i);
            }
        }
        state.Enabled = false;
    }

    private void SpawnTrain(ref SystemState state, Entity stationEntity, Station station, Config config, int systemId)
    {
        
            foreach (var platformChild in SystemAPI.GetBuffer<Child>(stationEntity))
            {
                if (state.EntityManager.HasComponent<Platform>(platformChild.Value))
                {
                    var platform = state.EntityManager.GetComponentData<Platform>(platformChild.Value);

                    var train = state.EntityManager.Instantiate(config.TrainPrefab);
                    var platfomTransform = state.EntityManager.GetComponentData<WorldTransform>(platformChild.Value);
                    var lt = LocalTransform.FromPosition(new float3(-51f, 0, -14f) + new float3(config.StationsOffset * station.Id, 0, config.LineOffset * station.SystemId + platform.Id * -50f));
                    state.EntityManager.SetComponentData<WorldTransform>(train, new WorldTransform { Position = lt.Position, Scale = 1 });
                    
                    

                    var trainComponent = state.EntityManager.GetComponentData<Train>(train);
                    //trainComponent.Line = station.Line;
                    foreach (var (line, lineEntity) in SystemAPI.Query<Line>().WithEntityAccess())
                    {
                        if (line.Id == platform.Id && line.SystemId == systemId)
                        {
                            trainComponent.Line = lineEntity;
                            // TODO: colors need to be set on specific mesh entities that are nested children of train
                            //SystemAPI.SetComponent(train, new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)line.LineColor });
                        }
                    }
                    state.EntityManager.SetComponentData<Train>(train, trainComponent);
                }
            }
        
    }
}

