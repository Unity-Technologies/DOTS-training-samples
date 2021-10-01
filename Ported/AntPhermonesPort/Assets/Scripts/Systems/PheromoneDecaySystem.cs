using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
struct DecayJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public DynamicBuffer<PheromoneMap> pheromoneMap;
    public float decayRate;

    public void Execute(int index)
    {
        ref PheromoneMap item = ref pheromoneMap.ElementAt(index);
        item.Intensity *= decayRate;
    }
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class PheromoneDecaySystem : SystemBase
{
    private UnityEngine.Texture2D texture;

    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();

        Entity pheromoneMapEntity = GetSingletonEntity<PheromoneMap>();
        var pheromoneMap = EntityManager.GetBuffer<PheromoneMap>(pheromoneMapEntity);

        DecayJob job = new DecayJob { pheromoneMap = pheromoneMap, decayRate = config.PheromoneDecayRate };

        Dependency = job.Schedule(pheromoneMap.Length, config.CellMapResolution, Dependency);

    }
}
