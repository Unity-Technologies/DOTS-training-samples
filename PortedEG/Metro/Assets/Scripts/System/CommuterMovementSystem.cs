using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Unity.Burst;


[BurstCompile]
// IJobEntity relies on source generation to implicitly define a query from the signature of the Execute function.
partial struct CommuterMovementJob : IJobEntity
{
    // A regular EntityCommandBuffer cannot be used in parallel, a ParallelWriter has to be explicitly used.
    public EntityCommandBuffer.ParallelWriter ECB;
    // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
    public float DeltaTime;

    // The ChunkIndexInQuery attributes maps the chunk index to an int parameter.
    // Each chunk can only be processed by a single thread, so those indices are unique to each thread.
    // They are also fully deterministic, regardless of the amounts of parallel processing happening.
    // So those indices are used as a sorting key when recording commands in the EntityCommandBuffer,
    // this way we ensure that the playback of commands is always deterministic.

    //void Execute([ChunkIndexInQuery] int chunkIndex, ref CommuterMovementAspect commuter)
    //{
    //    commuter.Position += commuter.Speed * DeltaTime;
    //}

    void Execute(ref CommuterMovementAspect commuter)
    {
        //commuter.Position += commuter.Speed * DeltaTime;

        //commuter.Scale = 10.0f;
    }
}

[BurstCompile]
[UpdateAfter(typeof(CommuterStateUpdateSystem))]
partial struct CommuterMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var job = new CommuterMovementJob
        {
            // Note the function call required to get a parallel writer for an EntityCommandBuffer.
            ECB = ecb.AsParallelWriter(),
            // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        job.ScheduleParallel();
    }
}
