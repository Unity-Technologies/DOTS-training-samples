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
        state.RequireForUpdate<FireTemperature>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var settings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();
        
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
                        var query = SystemAPI.QueryBuilder().WithAll<WaterCell, LocalToWorld>().Build();
                        SystemUtilities.GetNearestWaterPosition(nextPosition.ValueRO.Value, in settings, in query, out var waterPosition);
                        nextPosition.ValueRW.Value = waterPosition;
                        state.EntityManager.SetComponentEnabled<NextPosition>(omniEntity, true);
                        omniState.ValueRW.Value = OmniStates.FillingBucket;
                    }
                    break;
                case OmniStates.FillingBucket:
                    if (!IsMoving(ref state, omniEntity))
                    {
                        SystemUtilities.GetNearestFirePosition(nextPosition.ValueRO.Value, in settings, in temperatures, out var firePos);
                        nextPosition.ValueRW.Value = firePos;
                        state.EntityManager.SetComponentEnabled<NextPosition>(omniEntity, true);
                        omniState.ValueRW.Value = OmniStates.MovingToFire;
                    }                    
                    break;
                case OmniStates.MovingToFire:
                    if (!IsMoving(ref state, omniEntity))
                    {
                        var omniPosition = nextPosition.ValueRO.Value;
                        SystemUtilities.PutoutFire(omniPosition, in settings, ref temperatures);
                        omniState.ValueRW.Value = OmniStates.MovingToBucket;
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
}