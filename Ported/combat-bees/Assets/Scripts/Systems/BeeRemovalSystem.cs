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
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in LocalToWorldTransform transform, in BeeProperties beeProperties)
    {
        if (beeProperties.CarriedFood != Entity.Null && FoodComponentLookup.HasComponent(beeProperties.CarriedFood))
        {
            ECB.SetComponent(chunkIndex, beeProperties.CarriedFood, new Food{CarrierBee = Entity.Null});
            ECB.AddComponent<UnmatchedFood>(chunkIndex, beeProperties.CarriedFood);
        }

        if (transform.Value.Position.y >= FieldArea.Min.y)
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