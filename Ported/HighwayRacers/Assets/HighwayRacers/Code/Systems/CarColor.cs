using Aspects;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[WithAll(typeof(Car))]
[BurstCompile]
public partial struct CarColorJob : IJobEntity
{

    public EntityCommandBuffer.ParallelWriter EntityParalleWriter;
    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int entityQueryIndex, ref CarAspect Car)
    {
        var speedDifferential = Car.Speed;
        float4 color = new float4(0.5f, 0.5f, 0.5f, 1f);
        if (speedDifferential > 0.03f)
        {
            color = new float4(0, 1, 0, 1);
        }
        else if (speedDifferential < 0.02f)
        {
            color = new float4(1, 0, 0, 1);
        }

        EntityParalleWriter.SetComponent(entityQueryIndex, Car.Self,
        new URPMaterialPropertyBaseColor
        {
            Value = color
        });
    }
}
[BurstCompile]
public partial struct CarColor : ISystem
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
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        var writer = ecb.AsParallelWriter();

        var carColorJob = new CarColorJob
        {
            EntityParalleWriter = writer
        };
        var jobHandler = carColorJob.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandler;
        jobHandler.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();

    }
}
