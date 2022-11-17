using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct ZombieMovementSystem : ISystem
{
    private int _gridSize;
    private bool _initialized;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
        state.RequireForUpdate<Trajectory>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!_initialized)
        {
            _initialized = true;
            var config = SystemAPI.GetSingleton<GameConfig>();
            _gridSize = config.mazeSize;
        }

        float deltaTime = SystemAPI.Time.DeltaTime;
        
        new PathFollowJob
        {
            deltaTime = deltaTime,
            gridSize = _gridSize
        }.ScheduleParallel();
        
        var transformFromEntity = SystemAPI.GetComponentLookup<LocalToWorldTransform>(true);
        
        // Get an EntityCommandBuffer from the BeginSimulationEntityCommandBufferSystem.
        var ecbSingleton = SystemAPI.GetSingleton<
            BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var ecb2 = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        new HunterPlayerJob
        {
            Ecb = ecb.AsParallelWriter(),
            targetTransform = transformFromEntity,
        }.ScheduleParallel();
        
        //hunterPlayerJobHandle.Complete();
        
        new PillHunterJob
        {
            Ecb = ecb2.AsParallelWriter(),
            targetTransform = transformFromEntity,
        }.ScheduleParallel();

        //pillHunterJobHandle.Complete();
    }
}

[WithAll(typeof(Speed))]
[WithAll(typeof(PositionInPath))]
[BurstCompile]
public partial struct PathFollowJob : IJobEntity
{
    public int gridSize;
    public float deltaTime;

    [BurstCompile]
    public void Execute(Entity entity, DynamicBuffer<Trajectory> trajectory, TransformAspect transform, Speed speed, ref PositionInPath pathIndex)
    {
        if(trajectory.Length < 1)
            return;
        
        // Ignore starting position in path
        if (pathIndex.currentIndex == 0)
            pathIndex.currentIndex = 1;

        if (pathIndex.currentIndex < trajectory.Length)
        {
            float3 targetPos = MazeUtils.PositionFromIndex(trajectory[pathIndex.currentIndex], gridSize);

            float3 direction = math.normalize(targetPos - transform.Position);

            transform.Position += direction * speed.speed * deltaTime;
            
            if (math.distancesq(transform.Position, targetPos) < 0.1f)
            {
                pathIndex.currentIndex++;
            }
        }
    }
}

[WithAll(typeof(PlayerHunter))]
[WithAll(typeof(HunterTarget))]
[BurstCompile]
public partial struct HunterPlayerJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalToWorldTransform> targetTransform;
    public EntityCommandBuffer.ParallelWriter Ecb;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref HunterTarget target, PlayerHunter playerHunter)
    {
        target.position = targetTransform[playerHunter.player].Value.Position;
        //TODO set logic to recalculate path if the player is not in the path.
    }
}

[WithAll(typeof(PillHunter))]
[WithAll(typeof(HunterTarget))]
[BurstCompile]
public partial struct PillHunterJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalToWorldTransform> targetTransform;
    public EntityCommandBuffer.ParallelWriter Ecb;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref HunterTarget target)
    {
        var pillPosition = target.position;
        var currentPos = targetTransform[entity].Value.Position;
        float radius = 0.4f;
        
        if (math.distancesq(pillPosition, currentPos) < radius * radius)
        {
            Ecb.SetComponentEnabled<NeedUpdateTarget>(chunkIndex, entity, true);
        }   
    }
}