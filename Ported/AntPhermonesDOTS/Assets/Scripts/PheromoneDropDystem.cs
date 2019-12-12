using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(MapSystem))]
public class PheromoneDropSystem : JobComponentSystem
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
        
        Map map = GetSingleton<Map>();
        Entity mapEntity = GetSingletonEntity<Map>();
        DynamicBuffer<PheromoneBufferElement> dynamicBuffer = EntityManager.GetBuffer<PheromoneBufferElement>(mapEntity);
        if (dynamicBuffer.Length == 0)
        {
            dynamicBuffer = EntityManager.GetBuffer<PheromoneBufferElement>(mapEntity);
            dynamicBuffer.ResizeUninitialized(map.Size * map.Size);
            
            // zero the array
            for (int i = 0; i < dynamicBuffer.Length; ++i)
            {
                var v = dynamicBuffer[i];
                v.Value = 0;
                dynamicBuffer[i] = v;
            }
        }
        
        // Pheromone map get pheromones from ants.
        var parallel = dynamicBuffer.AsNativeArray();
        var handle = Entities
            .WithName("PheromoneDropSystem")
            .WithNativeDisableParallelForRestriction(parallel)
            .WithAll<AntTag>()
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation position) =>
            {
                // Find ant position in map.
                var positive = position.Value.xy + map.Size / 2; 
                int2 index = new int2(positive.xy);
                int bufferIndex = index.x * map.Size + index.y;
                var v = parallel[bufferIndex];
                // TODO should be using atomics like this
                //System.Threading.Interlocked.Increment(v.Value);
                // but once each texel in the pheromone map gets a
                // list of ants there will not be any threading issues.
                v.Value++; // leave some pheromones.
                parallel[bufferIndex] = v;
            }).Schedule(inputDeps);
        
        return JobHandle.CombineDependencies(handle, inputDeps);
    }
}