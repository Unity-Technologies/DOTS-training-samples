using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct OmniUpdateSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var random = Random.CreateFromIndex((uint)SystemAPI.Time.ElapsedTime);
        foreach(var (omniState, nextPosition, omniEntity) in 
                SystemAPI.Query<
                    RefRW<OmniState>, RefRW<NextPosition>>()
                    .WithEntityAccess()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
        {
            switch (omniState.ValueRW.Value)
            {
                case OmniStates.Idle:
                    var bucketPosition = GetRandomBucketPosition(ref state, ref random);
                    nextPosition.ValueRW.Value = bucketPosition;
                    state.EntityManager.SetComponentEnabled<NextPosition>(omniEntity, true);
                    omniState.ValueRW.Value = OmniStates.MovingToBucket;
                    break;
                case OmniStates.MovingToBucket:
                    if (!IsMoving(ref state, omniEntity))
                    {
                        var waterPosition = GetRandomWaterPosition(ref state, ref random);
                        nextPosition.ValueRW.Value = waterPosition;
                        state.EntityManager.SetComponentEnabled<NextPosition>(omniEntity, true);
                        omniState.ValueRW.Value = OmniStates.FillingBucket;
                    }
                    break;
                case OmniStates.FillingBucket:
                    if (!IsMoving(ref state, omniEntity))
                    {
                        var firePos = GetNearestFirePosition(ref state, nextPosition.ValueRO.Value);
                        nextPosition.ValueRW.Value = firePos;
                        state.EntityManager.SetComponentEnabled<NextPosition>(omniEntity, true);
                        omniState.ValueRW.Value = OmniStates.MovingToFire;
                    }                    
                    break;
                case OmniStates.MovingToFire:
                    if (!IsMoving(ref state, omniEntity))
                    {
                        var omniPosition = nextPosition.ValueRO.Value;
                        var settings = SystemAPI.GetSingleton<GameSettings>();
                        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();
                        SystemUtilities.PutoutFire(omniPosition, in settings, ref temperatures);
                        omniState.ValueRW.Value = OmniStates.Idle;
                    }
                    break;
            }
        }
    }

    bool IsMoving(ref SystemState state, Entity entity)
    {
        // If NextPosition is enabled, it means that the entity is moving.
        return state.EntityManager.IsComponentEnabled<NextPosition>(entity);
    }

    float2 GetRandomBucketPosition(ref SystemState state, ref Random random)
    {
        var query = SystemAPI.QueryBuilder().WithAll<BucketData, LocalToWorld>().Build();
        var transforms = query.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
        var randomIndex = random.NextInt(0, transforms.Length);
        return transforms[randomIndex].Position.xz;
    }
    
    float2 GetRandomWaterPosition(ref SystemState state, ref Random random)
    {
        var query = SystemAPI.QueryBuilder().WithAll<WaterCell, LocalToWorld>().Build();
        var transforms = query.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
        var randomIndex = random.NextInt(0, transforms.Length);
        return transforms[randomIndex].Position.xz;
    }    
    
    float2 GetNearestFirePosition(ref SystemState state, float2 currentPosition)
    {
        var settings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();

        var cols = settings.RowsAndColumns;
        var size = settings.Size;
        
        var closestPos = new float2();
        var closestDist = float.MaxValue;
        
        for (var i = 0; i < size; i++)
        {
            if (temperatures[i] <= 0f) continue;
            
            var firePosition = new float2((i % cols) * settings.DefaultGridSize, (i / cols) * settings.DefaultGridSize);
            var dist = math.distancesq(currentPosition, firePosition);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPos = firePosition;
            }
        }

        return closestPos;
    }    
}