using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
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
        
        foreach(var (omniState, omniData, nextPosition, omniEntity) in 
                SystemAPI.Query<
                    RefRW<OmniState>, RefRW<OmniData>, RefRW<NextPosition>>()
                    .WithEntityAccess()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
        {
            var random = Random.CreateFromIndex((uint)(omniEntity.Index + (int)(SystemAPI.Time.DeltaTime * 1000f)));
            
            switch (omniState.ValueRW.Value)
            {
                case OmniStates.Idle:
                    if (!IsMoving(ref state, omniEntity))
                        ChangeToMoveToBucket(ref state, ref random, ref omniState.ValueRW, ref nextPosition.ValueRW, omniEntity);
                    break;
                case OmniStates.MovingToBucket:
                    if (!IsMoving(ref state, omniEntity))
                        ChangeToFillingBucket(ref state, in settings, ref omniState.ValueRW, ref omniData.ValueRW, ref nextPosition.ValueRW, omniEntity);
                    break;
                case OmniStates.FillingBucket:
                    if (!IsMoving(ref state, omniEntity))
                        ChangeToMoveToFire(ref state, in settings, in temperatures, ref omniState.ValueRW, ref omniData.ValueRW, ref nextPosition.ValueRW, omniEntity);            
                    break;
                case OmniStates.MovingToFire:
                    if (IsMoving(ref state, omniEntity))
                    {
                        if (!IsFireActive(nextPosition.ValueRO.Value, temperatures, settings.DefaultGridSize, settings.RowsAndColumns))
                        {
                            omniState.ValueRW.Value = OmniStates.FillingBucket;
                            state.EntityManager.SetComponentEnabled<NextPosition>(omniEntity, false);
                        }
                    }
                    else
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

    void ChangeToMoveToBucket(ref SystemState state, 
        ref Random random, 
        ref OmniState omniState, 
        ref NextPosition nextPosition, 
        Entity omniEntity)
    {
        var bucketPosition = GetRandomBucketPosition(ref state, ref random);
        if (math.abs(bucketPosition.x - float.MinValue) < math.EPSILON)
            return;
                    
        nextPosition.Value = bucketPosition;
        state.EntityManager.SetComponentEnabled<NextPosition>(omniEntity, true);
        omniState.Value = OmniStates.MovingToBucket;        
    }

    void ChangeToFillingBucket(ref SystemState state, 
        in GameSettings settings,
        ref OmniState omniState,
        ref OmniData omniData, 
        ref NextPosition nextPosition, 
        Entity omniEntity)
    {
        omniData.HasBucket = true;
                        
        var query = SystemAPI.QueryBuilder().WithAll<WaterCell, LocalToWorld>().Build();
        SystemUtilities.GetNearestWaterPosition(nextPosition.Value, in settings, in query, out var waterPosition);
        if (math.abs(waterPosition.x - float.MinValue) < math.EPSILON)
            return;

        nextPosition.Value = waterPosition;
        state.EntityManager.SetComponentEnabled<NextPosition>(omniEntity, true);
        omniState.Value = OmniStates.FillingBucket;        
    }

    bool IsFireActive(float2 firePosition, in DynamicBuffer<FireTemperature> temperatures, float gridSize, int gridCols)
    {
        var gridPos = SystemUtilities.GetGridPosition(firePosition, gridSize);
        var gridIndex = SystemUtilities.GetGridIndex(gridPos, gridCols);
        return temperatures[gridIndex].Value > 0f;
    }

    void ChangeToMoveToFire(ref SystemState state, 
        in GameSettings settings,
        in DynamicBuffer<FireTemperature> temperatures,
        ref OmniState omniState,
        ref OmniData omniData, 
        ref NextPosition nextPosition,
        Entity omniEntity)
    {
        SystemUtilities.GetNearestFirePosition(nextPosition.Value, in settings, in temperatures, out var firePos);
        if (math.abs(firePos.x - float.MinValue) < math.EPSILON)
        {
            omniState.Value = omniData.HasBucket ? OmniStates.MovingToBucket : OmniStates.Idle;
            return;
        }
                        
        nextPosition.Value = firePos;
        state.EntityManager.SetComponentEnabled<NextPosition>(omniEntity, true);
        omniState.Value = OmniStates.MovingToFire;        
    }

    float2 GetRandomBucketPosition(ref SystemState state, ref Random random)
    {
        var query = SystemAPI.QueryBuilder().WithAll<BucketData, LocalTransform>().Build();
        if (query.IsEmpty)
            return new float2(float.MinValue);
        
        var transforms = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        var randomIndex = random.NextInt(0, transforms.Length);
        return transforms[randomIndex].Position.xz;
    }
}