using Unity.Burst;
using Unity.Collections;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
    
[WithAll(typeof(Dead), typeof(BeeProperties))]
[BurstCompile]
public partial struct BeeRemovalJob : IJobEntity
{
    [ReadOnly] public AABB FieldArea;
    public EntityCommandBuffer.ParallelWriter ECB;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in LocalToWorldTransform transform)
    {

        if (transform.Value.Position.y >= FieldArea.Min.y)
        {
            ECB.DestroyEntity(chunkIndex, entity);
        }
        
    }
}

[BurstCompile]
partial struct BeeRemovalSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
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
         
        var beeRemovalJob = new BeeRemovalJob
        {
            FieldArea = config.fieldArea,
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        };
        beeRemovalJob.ScheduleParallel();
    }
}