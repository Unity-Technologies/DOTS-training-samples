using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

partial struct FoodTargetingJob : IJobEntity
{
    [ReadOnly]
    public NativeArray<LocalToWorldTransform> FoodTransforms;

    [ReadOnly]
    public NativeArray<Entity> FoodEntities;

    [ReadOnly] public float TimeElapsed;

    void Execute([ChunkIndexInQuery] int chunkIndex, in LocalToWorldTransform transform, ref BeeProperties beeProperties, ref Velocity velocity)
    {
        if (beeProperties.Target != Entity.Null)
        {
            return;
        }

        var rand = new Random((uint)TimeElapsed);
        var index = rand.NextInt(0, FoodTransforms.Length - 1);

        var deltaPosition = transform.Value.Position -
                            FoodTransforms[index].Value.Position;

        velocity.velocity = deltaPosition/math.sqrt(
            deltaPosition.x * deltaPosition.x + 
            deltaPosition.y * deltaPosition.y + 
            deltaPosition.z * deltaPosition.z);

        beeProperties.Target = FoodEntities[index];
    }
}

[BurstCompile]
partial struct BeeSystem : ISystem
{
    private EntityQuery _foodQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<Food, LocalToWorldTransform>();
        
        _foodQuery = state.GetEntityQuery(builder);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var foodTransforms = _foodQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var foodEntities = _foodQuery.ToEntityArray(Allocator.TempJob);

        var foodTargetingJob = new FoodTargetingJob
        {
            TimeElapsed = (float)(state.Time.ElapsedTime * 100000d),
            FoodTransforms = foodTransforms,
            FoodEntities = foodEntities
        };
        foodTargetingJob.ScheduleParallel();
        // NOTE: The following is the default pattern, you don't need to state it: (happens at line 57)
        // state.Dependency = foodTargetingJob.ScheduleParallel(state.Dependency);
        
        // NOTE: Implicitly contains dependency line 57
        // Probably dont need that assignment since no other job will risk using this array
        state.Dependency = foodTransforms.Dispose(state.Dependency);
    }
}