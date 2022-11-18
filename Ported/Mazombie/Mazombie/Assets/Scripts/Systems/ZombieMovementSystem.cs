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
        
        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();

        var grid = state.EntityManager.GetBuffer<GridCell>(gameConfigEntity);

        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // Get an EntityCommandBuffer from the BeginSimulationEntityCommandBufferSystem.
        var ecbSingleton = SystemAPI.GetSingleton<
            BeginSimulationEntityCommandBufferSystem.Singleton>();
        
        var pathFollowCb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var playerHunterCb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var pillHunterCb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new PathFollowJob
        {
            Ecb = pathFollowCb.AsParallelWriter(),
            grid = grid,
            deltaTime = deltaTime,
            gridSize = _gridSize
        }.ScheduleParallel();
        
        var transformFromEntity = SystemAPI.GetComponentLookup<LocalToWorldTransform>(true);

        new HunterPlayerJob
        {
            Ecb = playerHunterCb.AsParallelWriter(),
            targetTransform = transformFromEntity,
            gridSize = _gridSize
        }.ScheduleParallel();

        new PillHunterJob
        {
            gridSize = _gridSize,
            Ecb = pillHunterCb.AsParallelWriter(),
        }.ScheduleParallel();
    }
}

[WithAll(typeof(Speed))]
[WithAll(typeof(PositionInPath))]
[BurstCompile]
public partial struct PathFollowJob : IJobEntity
{
    public int gridSize;
    public float deltaTime;
    [ReadOnly]public DynamicBuffer<GridCell> grid;
    public EntityCommandBuffer.ParallelWriter Ecb;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, DynamicBuffer<Trajectory> trajectory, TransformAspect transform, Speed speed, ref PositionInPath pathIndex)
    {
        if(trajectory.Length < 1)
            return;

        // Ignore starting position in path
        if (pathIndex.currentIndex == 0)
            pathIndex.currentIndex = 1;

        if (pathIndex.currentIndex < trajectory.Length)
        {
            float3 targetPosition = MazeUtils.PositionFromIndex(trajectory[pathIndex.currentIndex], gridSize);
            int2 targetCell = MazeUtils.WorldPositionToGrid(targetPosition);
            int2 currentCell = MazeUtils.WorldPositionToGrid(transform.Position);
            int currentCellIndex = MazeUtils.CellIdxFromPos(currentCell.x, currentCell.y, gridSize);

            int2 gridDirection = targetCell - currentCell;
            float3 direction = math.normalize(targetPosition - transform.Position);

            bool blockedByMovingWall = false;
            
            // Check for moving walls
            switch (gridDirection.x)
            {
                case 0 when gridDirection.y == -1 && (grid[currentCellIndex].wallFlags & (1 << 1)) != 0:
                case 0 when gridDirection.y == 1 && (grid[currentCellIndex].wallFlags & (1 << 0)) != 0:
                case -1 when gridDirection.y == 0 && (grid[currentCellIndex].wallFlags & (1 << 3)) != 0:
                case +1 when gridDirection.y == 0 && (grid[currentCellIndex].wallFlags & (1 << 2)) != 0:
                    blockedByMovingWall = true;
                    break;
            }

            if (!blockedByMovingWall)
            {
                transform.Position += direction * speed.speed * deltaTime;
            
                if (math.distancesq(transform.Position, targetPosition) < 0.1f)
                {
                    pathIndex.currentIndex++;
                }   
            }
            else
            {
                Ecb.SetComponentEnabled<NeedUpdateTrajectory>(chunkIndex, entity, true);
            }
        }
        
        // Debug path
        // for (int i = 0; i < trajectory.Length-1; i++)
        // {
        //     float3 pos = MazeUtils.PositionFromIndex(trajectory[i], gridSize);
        //     float3 nextPos = MazeUtils.PositionFromIndex(trajectory[i+1], gridSize);
        //     Debug.DrawLine(pos, nextPos);
        // }
    }
}

[WithAll(typeof(PlayerHunter))]
[WithAll(typeof(HunterTarget))]
[BurstCompile]
public partial struct HunterPlayerJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalToWorldTransform> targetTransform;
    public EntityCommandBuffer.ParallelWriter Ecb;
    public int gridSize;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref HunterTarget target, PlayerHunter playerHunter, DynamicBuffer<Trajectory> trajectory)
    {
        target.position = targetTransform[playerHunter.player].Value.Position;

        int2 playerGridPosition = MazeUtils.WorldPositionToGrid(target.position);
        int2 finalPathPosition = MazeUtils.CellFromIndex(trajectory[trajectory.Length - 1], gridSize);

        if (playerGridPosition.x != finalPathPosition.x || playerGridPosition.y != finalPathPosition.y)
        {
            Ecb.SetComponentEnabled<NeedUpdateTrajectory>(chunkIndex, entity, true);
        }
    }
}

[WithAll(typeof(PillHunter))]
[WithAll(typeof(HunterTarget))]
[BurstCompile]
public partial struct PillHunterJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public int gridSize;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, TransformAspect transform, Entity entity, ref HunterTarget target, DynamicBuffer<Trajectory> trajectory)
    {
        var pillPosition = target.position;
        var currentPos = transform.Position;
        float radius = 0.4f;

        if (trajectory.Length > 1)
        {
            int2 pillGridPosition = MazeUtils.WorldPositionToGrid(target.position);
            int2 finalPathPosition = MazeUtils.CellFromIndex(trajectory[trajectory.Length - 1], gridSize);

            if (pillGridPosition.x != finalPathPosition.x || pillGridPosition.y != finalPathPosition.y)
            {
                Ecb.SetComponentEnabled<NeedUpdateTrajectory>(chunkIndex, entity, true);
            }
        }

        float distance = math.distancesq(pillPosition, currentPos);

        if (distance < radius * radius)
        {
            Ecb.SetComponentEnabled<NeedUpdateTarget>(chunkIndex, entity, true);
        }   
    }
}