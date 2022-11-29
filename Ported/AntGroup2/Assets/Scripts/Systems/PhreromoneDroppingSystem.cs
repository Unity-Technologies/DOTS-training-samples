using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct PheromoneDroppingSystem : ISystem
{
    private EntityQuery query;
    private EntityQuery query2;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //query = state.GetEntityQuery(typeof(TransformAspect)); // TODO: how to get the single pheromone map and all ants?? Two queries?
        //query2 = state.GetEntityQuery(typeof(DynamicBuffer<PheromoneMap>));
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
    }
}