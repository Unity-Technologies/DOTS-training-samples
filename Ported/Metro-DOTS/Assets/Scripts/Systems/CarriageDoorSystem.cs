using Metro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TrainSystemGroup))]
[UpdateAfter(typeof(TrainMoverSystem))]
public partial struct OpenDoorSystem : ISystem
{
    ComponentLookup<UnloadingComponent> _unloadingLookup;
    ComponentLookup<DepartingComponent> _departingLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<Door>();

        _unloadingLookup = state.GetComponentLookup<UnloadingComponent>();
        _departingLookup = state.GetComponentLookup<DepartingComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get an EntityCommandBuffer from the BeginSimulationEntityCommandBufferSystem.
        var ecbSingleton = SystemAPI.GetSingleton<
            BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        _unloadingLookup.Update(ref state);
        _departingLookup.Update(ref state);

        // Create the job.
        var openJob = new OpenDoorJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            Ecb = ecb.AsParallelWriter(),
            UnloadingLookup = _unloadingLookup,
            DepartingLookup = _departingLookup
        };
        var openJobHandle = openJob.Schedule(state.Dependency);

        // Schedule the job. Source generation creates and passes the query implicitly.
        state.Dependency = openJobHandle;
    }
}

[WithAll(typeof(Door), typeof(LocalTransform))]
[WithAny(typeof(UnloadingComponent), typeof(DepartingComponent))]
[BurstCompile]
public partial struct OpenDoorJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter Ecb;
    [ReadOnly] public ComponentLookup<UnloadingComponent> UnloadingLookup;
    [ReadOnly] public ComponentLookup<DepartingComponent> DepartingLookup;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Door door, ref LocalTransform transform)
    {
        if (UnloadingLookup.IsComponentEnabled(entity) && DepartingLookup.IsComponentEnabled(entity))
        {
            door.IsOpening ^= true;

            // Add a close delay
            door.Timer = door.IsOpening ? 0f : -1.5f;

            Ecb.SetComponentEnabled<UnloadingComponent>(chunkIndex, entity, door.IsOpening);
            Ecb.SetComponentEnabled<DepartingComponent>(chunkIndex, entity, !door.IsOpening);
        }

        door.Timer += DeltaTime;
        
        var lerp = math.clamp(door.Timer / Door.OpeningTime, 0f, 1f);

        transform.Position = door.IsOpening ? math.lerp(door.ClosedPosition, door.OpenPosition, lerp) : math.lerp(door.OpenPosition, door.ClosedPosition, lerp);
    }
}