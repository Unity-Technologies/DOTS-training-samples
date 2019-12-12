using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(PheromoneDropSystem))]
public class PheromoneDecaySystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem EntityCommandBufferSystem;
    private EntityQuery MapInitQuery;
    
    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        MapInitQuery = GetEntityQuery(ComponentType.ReadWrite<MapInitializedTag>());
        RequireSingletonForUpdate<Map>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (MapInitQuery.IsEmptyIgnoreFilter)
            return inputDeps;
        
        Entity mapEntity = GetSingletonEntity<Map>();
        DynamicBuffer<PheromoneBufferElement> pheromones = EntityManager.GetBuffer<PheromoneBufferElement>(mapEntity);
        if (pheromones.Length == 0)
        {
            // not ready yet
            return inputDeps;
        }
        
        Map map = GetSingleton<Map>();
        var LUT = GetBufferFromEntity<PheromoneBufferElement>();
        var handle = Entities
            .WithName("PheromoneDecaySystem")
            .WithNativeDisableParallelForRestriction(LUT)
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref Tile tile) =>
            {
                var parallel = LUT[mapEntity].AsNativeArray();
                
                // Each tile decays a chunk of the array.
                int begin = tile.Coordinates.x;
                int end = tile.Coordinates.y;
                for (int x = begin; x < end; ++x)
                {
                    var v = parallel[x];
                    v.Value -= map.TrailDecay; // Decay pheromones.
                    if (v.Value < 0.0f)
                    {
                        v.Value = 0.0f;
                    }
                    parallel[x] = v;
                }
            }).Schedule(inputDeps);
        
        return handle;
    }
}