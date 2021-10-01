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
        item.intensity.x *= decayRate;
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
        PheromoneMapHelper helper = new PheromoneMapHelper(EntityManager.GetBuffer<PheromoneMap>(pheromoneMapEntity), config.CellMapResolution, config.WorldSize);

        DecayJob job = new DecayJob { pheromoneMap = helper.pheromoneMap, decayRate = config.PheromoneDecayRate };

        Dependency = job.Schedule(helper.pheromoneMap.Length, config.CellMapResolution, Dependency);

// TODO: cleanup/remove
  //      helper.DecrementIntensity(config.PheromoneDecayRate);
    }
}
