using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
    
[WithAll(typeof(Dead))]
[BurstCompile]
public partial struct BeeRemovalJob : IJobEntity
{
    [ReadOnly] public AABB FieldArea;
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentLookup<Food> FoodComponentLookup;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Velocity velocity, in BeeProperties beeProperties)
    {
        if (beeProperties.CarriedFood != Entity.Null && FoodComponentLookup.HasComponent(beeProperties.CarriedFood))
        {
            ECB.AddComponent<UnmatchedFood>(chunkIndex, beeProperties.CarriedFood);
        }

        float k_eps = 1e-2f;
        var vel = velocity.Value;
        if (math.lengthsq(vel) < k_eps)
        {
            ECB.DestroyEntity(chunkIndex, entity);
        }
    }
}

[BurstCompile]
partial struct BeeRemovalSystem : ISystem
{
    private ComponentLookup<Food> _foodComponentLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
        _foodComponentLookup = state.GetComponentLookup<Food>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
         
        _foodComponentLookup.Update(ref state);
        
        var beeRemovalJob = new BeeRemovalJob
        {
            FieldArea = config.fieldArea,
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            FoodComponentLookup = _foodComponentLookup,
        };
        beeRemovalJob.ScheduleParallel();
    }
}