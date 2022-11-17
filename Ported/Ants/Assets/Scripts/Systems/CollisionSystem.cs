using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;

[UpdateAfter(typeof(AntMovementSystem))]
[BurstCompile]
public partial struct CollisionSystem : ISystem
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
        var config = SystemAPI.GetSingleton<Config>();
        var query = SystemAPI.QueryBuilder()
            .WithAll<Wall, LocalToWorldTransform>()
            .Build();

        var wallTransforms = query.ToComponentDataArray<LocalToWorldTransform>(state.WorldUpdateAllocator);
        
        new CollisionJob
        {
            config = config,
            wallTransforms = wallTransforms
        }.ScheduleParallel();
        
    }
    
}

[BurstCompile]
public partial struct CollisionJob : IJobEntity
{
    public Config config;
    [ReadOnly] public NativeArray<LocalToWorldTransform> wallTransforms;
    public void Execute([ChunkIndexInQuery] int chunkIndex, ref Ant ant, in TransformAspect antTransform)
    {
        if (IsOutsideBounds(antTransform.Position.x, antTransform.Position.y, config.MapSize))
        {
            ant.Angle += 90f;
            return;
        }

        foreach (var wallTransform in wallTransforms)
        {
            var sqrDistance =math.distancesq(wallTransform.Value.Position, antTransform.Position) ;
            if (sqrDistance <= wallTransform.Value.Scale) //COLLIDED
            {
                ant.Angle += 90f;
            }
        }
    }
    
    private bool IsOutsideBounds(float xPos, float yPos, int mapSize)
    {
        if (xPos < 1) return true;
        if (yPos < 1) return true;
        if (xPos >= mapSize - 1) return true;
        if (yPos >= mapSize - 1) return true;
        return false;
    }
}