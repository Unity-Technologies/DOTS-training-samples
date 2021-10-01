using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class PheromoneDecaySystem : SystemBase
{
    private UnityEngine.Texture2D texture;

    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();

        Entity pheromoneMapEntity = GetSingletonEntity<PheromoneMap>();
        PheromoneMapHelper helper = new PheromoneMapHelper(EntityManager.GetBuffer<PheromoneMap>(pheromoneMapEntity), config.CellMapResolution, config.WorldSize);

        helper.DecrementIntensity(config.PheromoneDecayRate);
    }
}
