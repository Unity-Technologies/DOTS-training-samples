using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PheromoneDropSystem))]
public class PheromoneDecaySystem : JobComponentSystem
{
    EntityQuery m_PheromoneMapQuery;
    EntityQuery m_MapQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_PheromoneMapQuery = GetEntityQuery(ComponentType.ReadWrite<PheromoneMapComponent>());
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var pheromoneMap = m_PheromoneMapQuery.GetSingleton<PheromoneMapComponent>();
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        return new Job
        {
            TrailDecay = map.TrailDecay,
            PheromoneMap = pheromoneMap.PheromoneMap,
        }.Schedule(map.MapSize, 64, inputDeps);
    }

    [BurstCompile]
    struct Job : IJobParallelFor
    {
        public float TrailDecay;
        public BlobAssetReference<PheromoneMap> PheromoneMap;
        public void Execute(int index)
        {
            PheromoneMap.Value.Map[index] *= TrailDecay;
        }
    }
}
