using Metro;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct OpenDoorSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get an EntityCommandBuffer from the BeginSimulationEntityCommandBufferSystem.
        var ecbSingleton = SystemAPI.GetSingleton<
            BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Create the job.
        var openJob = new OpenDoorJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            Ecb = ecb.AsParallelWriter()
        };
        var openJobHandle = openJob.Schedule(state.Dependency);

        // Schedule the job. Source generation creates and passes the query implicitly.
        state.Dependency = openJobHandle;
    }
}

[WithAll(typeof(Door), typeof(LocalTransform))]
[BurstCompile]
public partial struct OpenDoorJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter Ecb;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Door door, ref LocalTransform transform)
    {
        door.Timer += DeltaTime;

        var lerp = math.min(1f, door.Timer / Door.OpeningTime);
        transform.Position = door.IsOpening ? math.lerp(door.ClosedPosition, door.OpenPosition, lerp) : math.lerp(door.OpenPosition, door.ClosedPosition, lerp);

        if (lerp >= 1f)
        {
            door.Timer = 0f;
            door.IsOpening ^= true;
            Ecb.SetComponentEnabled<Door>(chunkIndex, entity, false);
        }
    }
}