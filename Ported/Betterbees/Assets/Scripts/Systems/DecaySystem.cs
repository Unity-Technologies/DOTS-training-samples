using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public partial struct DecaySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Store the destroy commands within a command buffer, this postpones structural changes while iterating through the query results
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var decayJob = new DecayJob()
        {
            dt = SystemAPI.Time.DeltaTime,
            ecb = ecb.AsParallelWriter()
        };
        decayJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct DecayJob : IJobEntity
{
    public float dt;
    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(ref DecayComponent decay, ref LocalTransform transform, Entity entity, [ChunkIndexInQuery] int chunkIndex)
    {
        transform.Scale -= decay.DecayRate * dt;

        if (transform.Scale <= 0.0f)
        {
            ecb.DestroyEntity(chunkIndex, entity);
        }
    }
}
