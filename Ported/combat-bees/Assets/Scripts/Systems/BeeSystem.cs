using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct BeePointToTargetJob : IJobEntity
{

    [ReadOnly]
    public ComponentLookup<LocalToWorldTransform> ComponentLookup;

    [ReadOnly] public float TimeStep;
    [ReadOnly] public float GravityDt;
    [ReadOnly] public float BeeSpeed;
    [ReadOnly] public float ObjectSize;

    private void Execute([ChunkIndexInQuery] int chunkIndex, in LocalToWorldTransform transform,
        ref BeeProperties beeProperties, ref Velocity velocity)
    {
        if (beeProperties.BeeMode != BeeMode.HeadTowardsResource)
        {
            return;
        }

        // Get latest position of target
        var newPosition = ComponentLookup.GetRefRO(beeProperties.Target).ValueRO.Value.Position;
        // work a small offset into the target position to account for object sizes
        newPosition.y += ObjectSize;

        beeProperties.TargetPosition = newPosition;

        var vel = MovementUtilities.ComputeTargetVelocity(transform.Value.Position, newPosition, GravityDt, TimeStep, BeeSpeed);
        velocity.Value = vel;
    }
}

partial struct BeeActionProcessingJob : IJob
{
    [ReadOnly]
    public NativeArray<LocalToWorldTransform> FoodTransforms;

    [ReadOnly]
    public NativeArray<Entity> FoodEntities;

    [ReadOnly]
    public NativeArray<LocalToWorldTransform> BeeTransforms;
    
    [ReadOnly]
    public NativeArray<BeeProperties> BeeProperties;
    
    [ReadOnly]
    public NativeArray<Entity> BeeEntities;

    public EntityCommandBuffer ECB;

    // [INFO/NICE TO SEE TRACES]Original parallel -> all bees in parallel, match to random food.
    // private void Execute([ChunkIndexInQuery] int chunkIndex, in LocalToWorldTransform transform, ref BeeProperties beeProperties, ref Velocity velocity)
    // {
    //     if (beeProperties.Target != Entity.Null)
    //     {
    //         return;
    //     }
    //
    //     var rand = new Random((uint)TimeElapsed);
    //     var index = rand.NextInt(0, FoodTransforms.Length - 1);
    //
    //     var deltaPosition = transform.Value.Position -
    //                         FoodTransforms[index].Value.Position;
    //
    //     velocity.velocity = deltaPosition/math.sqrt(
    //         deltaPosition.x * deltaPosition.x + 
    //         deltaPosition.y * deltaPosition.y + 
    //         deltaPosition.z * deltaPosition.z);
    //
    //     beeProperties.Target = FoodEntities[index];
    // }

    public void Execute()
    {
        for (int i = 0; i < BeeProperties.Length; i++)
        {
            switch (BeeProperties[i].BeeMode)
            {
                case BeeMode.Attack:
                    // Currently attacking a bee
                    break;
                case BeeMode.Idle:
                    // Switch to Attack or FindResource
                    ECB.SetComponent(BeeEntities[i], new BeeProperties
                    {
                        BeeMode = BeeMode.FindResource,
                        Aggressivity = BeeProperties[i].Aggressivity,
                        Target = Entity.Null
                    });
                    break;
                case BeeMode.FindResource:
                    // Pick a target resource and switch to HeadTowardsResource mode
                    FindTargetForBee(i);
                    break;
                case BeeMode.HeadTowardsResource:
                    TryPickupTargetResource(i);
                    // Head towards targeted resource + pickup on reaching it
                    break;
                case BeeMode.MoveResourceBackToNest:
                    // Return to base with resource / No action
                    break;
            }
        }
    }

    private void FindTargetForBee(int i)
    {
        // No more food: stop
        if (i >= FoodEntities.Length)
        {
            return;
        }
        
        // Inverting index for food (temp hack, all bees spawn on food with match index)
        var foodIndex = FoodEntities.Length - (1 + i);
        
        ECB.SetComponent(BeeEntities[i], new BeeProperties
        {
            Aggressivity = BeeProperties[i].Aggressivity,
            Target = FoodEntities[foodIndex],
            BeeMode = BeeMode.HeadTowardsResource,
            TargetPosition = FoodTransforms[foodIndex].Value.Position
        });
        
        ECB.RemoveComponent<UnmatchedFood>(FoodEntities[foodIndex]);
    }

    private void TryPickupTargetResource(int i)
    {
        // Check distance to target, if within a certain range, pickup the food.
        var deltaPosition = BeeProperties[i].TargetPosition - BeeTransforms[i].Value.Position;
        if (math.lengthsq(deltaPosition) < 0.01f)
        {
            ECB.SetComponent(BeeEntities[i], new BeeProperties
            {
                BeeMode = BeeMode.MoveResourceBackToNest,
                Aggressivity = BeeProperties[i].Aggressivity,
                Target = BeeProperties[i].Target,
                TargetPosition = BeeTransforms[i].Value.Position
            });
            ECB.SetComponent(BeeProperties[i].Target, new Food
            {
                CarrierBee = BeeEntities[i]
            });
        }
    }
}

[BurstCompile]
partial struct BeeSystem : ISystem
{
    private EntityQuery _foodQuery;
    private EntityQuery _beeQuery;
    private ComponentLookup<LocalToWorldTransform> _transformComponentLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Build queries
        var foodQueryBuilder = new EntityQueryBuilder(Allocator.Temp);
        foodQueryBuilder.WithAll<UnmatchedFood, Food, LocalToWorldTransform>();
        _foodQuery = state.GetEntityQuery(foodQueryBuilder);

        var beeQueryBuilder = new EntityQueryBuilder(Allocator.Temp);
        beeQueryBuilder.WithAll<BeeProperties, LocalToWorldTransform>();
        _beeQuery = state.GetEntityQuery(beeQueryBuilder);

        _transformComponentLookup = state.GetComponentLookup<LocalToWorldTransform>();

        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // TODO further breakup into separate jobs and only run the necessary ones. Before there was a check for valid food at this point which skipped job if none were found. Same concept, just better.

        var config = SystemAPI.GetSingleton<BeeConfig>();

        // Prepare bee action selection job
        // Food structures
        var foodEntities = _foodQuery.ToEntityArray(Allocator.TempJob);
        var foodTransforms = _foodQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        
        // Bee structures
        var beeProperties = _beeQuery.ToComponentDataArray<BeeProperties>(Allocator.TempJob);
        var beeTransforms = _beeQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var beeEntities = _beeQuery.ToEntityArray(Allocator.TempJob);
        
        // Command buffer
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        // BeeAction job instantiate and schedule
        var beeActionProcessingJob = new BeeActionProcessingJob
        {
            FoodTransforms = foodTransforms,
            FoodEntities = foodEntities,
            BeeProperties = beeProperties,
            BeeTransforms = beeTransforms,
            BeeEntities = beeEntities,
            ECB = ecb
        };
        state.Dependency = beeActionProcessingJob.Schedule(state.Dependency);
        // [IJobEntity only?]
        // NOTE: The following is the default pattern, you don't need to state it: (happens at line 57)
        // state.Dependency = foodTargetingJob.ScheduleParallel(state.Dependency);
        
        // NOTE: Implicitly contains dependency line 57
        // [QUESTION]Combine dependencies + dispose once == better?
        // [QUESTION]Changed from IJobEntity to IJob. Effect on auto-dependency management?
        state.Dependency = foodTransforms.Dispose(state.Dependency);
        state.Dependency = foodEntities.Dispose(state.Dependency);
        state.Dependency = beeProperties.Dispose(state.Dependency);
        state.Dependency = beeEntities.Dispose(state.Dependency);
        state.Dependency = beeTransforms.Dispose(state.Dependency);
        
        
        // Point towards target job
        _transformComponentLookup.Update(ref state);
        var beeTargetingJob = new BeePointToTargetJob // IJobEntity
        {
            ComponentLookup = _transformComponentLookup,
            TimeStep = state.Time.DeltaTime,
            GravityDt = config.gravity * state.Time.DeltaTime,
            BeeSpeed = config.beeSpeed,
            ObjectSize = config.objectSize
        };
        state.Dependency = beeTargetingJob.ScheduleParallel(state.Dependency);
    }
}