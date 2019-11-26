using Unity.Burst;
using Unity.Collections;
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
        m_PheromoneMapQuery = GetEntityQuery(ComponentType.ReadWrite<PheromoneBuffer>());
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var pheromoneFromEntity = GetBufferFromEntity<PheromoneBuffer>();
        var pheromoneMap = pheromoneFromEntity[m_PheromoneMapQuery.GetSingletonEntity()];
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        return new DecayJob
        {
            TrailDecay = map.TrailDecay,
            PheromoneMap = pheromoneMap.AsNativeArray()
        }.Schedule(pheromoneMap.Length, 256, inputDeps);
    }

    [BurstCompile]
    struct DecayJob : IJobParallelFor
    {
        public float TrailDecay;
        public NativeArray<PheromoneBuffer> PheromoneMap;
        public void Execute(int index)
        {
            PheromoneMap[index] *= TrailDecay;
        }
    }
}
