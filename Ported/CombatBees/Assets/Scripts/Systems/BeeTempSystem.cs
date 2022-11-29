using System;
using System.Numerics;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
// IJobEntity relies on source generation to implicitly define a query from the signature of the Execute function.
partial struct MoveBeeJob : IJobEntity
{
    // A regular EntityCommandBuffer cannot be used in parallel, a ParallelWriter has to be explicitly used.
    public EntityCommandBuffer.ParallelWriter ECB;
    // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
    public double DeltaTime;

    // The ChunkIndexInQuery attributes maps the chunk index to an int parameter.
    // Each chunk can only be processed by a single thread, so those indices are unique to each thread.
    // They are also fully deterministic, regardless of the amounts of parallel processing happening.
    // So those indices are used as a sorting key when recording commands in the EntityCommandBuffer,
    // this way we ensure that the playback of commands is always deterministic.
    void Execute([ChunkIndexInQuery] int chunkIndex, BeeTempAspect beeTemp)
    {
        var gravity = new float3(0.0f, -9.82f, 0.0f);
        var invertY = new float3(1.0f, -1.0f, 1.0f);

        //beeTemp.Position += beeTemp.Speed * DeltaTime;
        // if (beeTemp.Position.y < 0.0f)
        // {
        //     beeTemp.Position *= invertY;
        //     beeTemp.Speed *= invertY * 0.8f;
        // }

        // beeTemp.Speed += gravity * DeltaTime;

        // var speed = math.lengthsq(beeTemp.Speed);
        // if (speed < 0.1f) ECB.DestroyEntity(chunkIndex, beeTemp.Self);
        float3 newPos = new float3((float)Math.Sin(DeltaTime), beeTemp.Position.y, (float)Math.Cos(DeltaTime));
        beeTemp.Speed = 0.5f;
        beeTemp.Position = newPos;
    }
}

[BurstCompile]
partial struct BeeTempSystem : ISystem
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
        //var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        //var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var deltaTime = SystemAPI.Time.ElapsedTime;
        
        foreach (var beeTemp in SystemAPI.Query<BeeTempAspect>())
        {
            float3 newPos = new float3((float)Math.Sin(deltaTime), beeTemp.Position.y, (float)Math.Cos(deltaTime));
            beeTemp.Speed = 0.5f;
            beeTemp.Position = newPos;
        }

        // var moveBeeJob = new MoveBeeJob
        // {
        //     // Note the function call required to get a parallel writer for an EntityCommandBuffer.
        //     ECB = ecb.AsParallelWriter(),
        //     // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
        //     DeltaTime = SystemAPI.Time.ElapsedTime,//SystemAPI.Time.DeltaTime
        // };
        // moveBeeJob.ScheduleParallel();
    }
}