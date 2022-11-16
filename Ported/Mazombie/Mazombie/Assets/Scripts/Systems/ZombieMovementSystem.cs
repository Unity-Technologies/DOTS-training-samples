using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

        new MoveZombieJob
        {
            deltaTime = deltaTime,
            gridSize = _gridSize
        }.ScheduleParallel();
    }
}

[WithAll(typeof(Speed))]
[WithAll(typeof(PositionInPath))]
[BurstCompile]
public partial struct MoveZombieJob : IJobEntity
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

            // TODO optimize
            if (math.distancesq(transform.Position, targetPos) < 0.1f)
            {
                pathIndex.currentIndex++;
            }
        }
    }
}
