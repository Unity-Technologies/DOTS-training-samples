using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct BeePointToTargetJob : IJobEntity
{
    [ReadOnly]
    public ComponentLookup<LocalToWorldTransform> ComponentLookup;

    [ReadOnly] public float TimeStep;
    [ReadOnly] public float GravityDt;
    [ReadOnly] public float BeeSpeed;
    [ReadOnly] public float ObjectSize;

    public EntityCommandBuffer.ParallelWriter ECB;

    private void Execute([ChunkIndexInQuery] int chunkIndex, in LocalToWorldTransform transform,
        ref BeeProperties beeProperties, ref Velocity velocity)
    {
        if (beeProperties.BeeMode != BeeMode.HeadTowardsResource && beeProperties.BeeMode != BeeMode.MoveResourceBackToNest)
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
        
        if (beeProperties.BeeMode == BeeMode.MoveResourceBackToNest)
        {
            ECB.SetComponent(chunkIndex, beeProperties.CarriedFood, new Velocity
            {
                Value = vel
            });
        }
    }
}

partial struct BeeActionProcessingJob : IJob
{
    [ReadOnly]
    public NativeArray<LocalToWorldTransform> FoodTransforms;

    [ReadOnly]
    public NativeArray<Entity> FoodEntities;

    [ReadOnly]
    public NativeArray<Food> FoodProperties;

    [ReadOnly]
    public NativeArray<LocalToWorldTransform> BeeTransforms;
    
    [ReadOnly]
    public NativeArray<BeeProperties> BeeProperties;
    
    [ReadOnly]
    public NativeArray<Entity> BeeEntities;
    
    [ReadOnly]
    public NativeArray<LocalToWorldTransform> NestTransform;
    
    [ReadOnly]
    public NativeArray<Entity> NestEntities;

    [ReadOnly]
    public NativeArray<Area> NestAreas;

    [ReadOnly]
    public Random Random;
    
    [ReadOnly]
    public ComponentLookup<Food> FoodComponentLookup;

    [ReadOnly]
    public int Faction;

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
                    TryDropoffResource(i);
                    // Return to base with resource / No action
                    break;
            }
        }
    }

    private void TryDropoffResource(int index)
    {
        // Check if in bounds of nest
        if (!NestAreas[0].Value.Contains(BeeTransforms[index].Value.Position))
        {
            return;
        }
        
        ECB.SetComponent(BeeProperties[index].CarriedFood, new Velocity
        {
            Value = float3.zero
        });
        ECB.SetComponent(BeeProperties[index].CarriedFood, new Food
        {
            CarrierBee = Entity.Null
        });
        ECB.SetSharedComponent(BeeProperties[index].CarriedFood, new Faction
        {
            Value = Faction
        });
        
        ECB.SetComponent(BeeEntities[index], new BeeProperties
        {
            BeeMode = BeeMode.Idle,
            Target = Entity.Null,
            CarriedFood = BeeProperties[index].Target
        });
    }

    private void FindTargetForBee(int index)
    {
        if (FoodProperties.Length == 0)
        {
            return;
        }
        
        var foodIndex = Random.NextInt(0, FoodEntities.Length - 1);
        // Food already spoken for, stop, retry next frame.
        if (FoodProperties[foodIndex].CarrierBee != Entity.Null)
        {
            return;
        }
        
        ECB.SetComponent(BeeEntities[index], new BeeProperties
        {
            Aggressivity = BeeProperties[index].Aggressivity,
            Target = FoodEntities[foodIndex],
            BeeMode = BeeMode.HeadTowardsResource,
            TargetPosition = FoodTransforms[foodIndex].Value.Position
        });
        
        ECB.RemoveComponent<UnmatchedFood>(FoodEntities[foodIndex]);
    }

    private void TryPickupTargetResource(int index)
    {
        // [DECISION REQUIRED]Check this now or on close enough to pickup?
        // If food is already spoken for, cancel, go back to idle
        var foodCarrier = FoodComponentLookup.GetRefRO(BeeProperties[index].Target).ValueRO.CarrierBee;
        if (foodCarrier != Entity.Null)
        {
            ECB.SetComponent(BeeEntities[index], new BeeProperties
            {
                BeeMode = BeeMode.Idle,
                Aggressivity = BeeProperties[index].Aggressivity,
                Target = NestEntities[0],
                TargetPosition = NestTransform[0].Value.Position
            });
            return;
        }
        
        // Check distance to target, if within a certain range, pickup the food.
        var deltaPosition = BeeProperties[index].TargetPosition - BeeTransforms[index].Value.Position;
        if (math.lengthsq(deltaPosition) < 0.01f)
        {
            ECB.SetComponent(BeeEntities[index], new BeeProperties
            {
                BeeMode = BeeMode.MoveResourceBackToNest,
                TargetPosition = NestTransform[0].Value.Position,
                CarriedFood = BeeProperties[index].Target,
                Target = NestEntities[0]
            });
            ECB.SetComponent(BeeProperties[index].Target, new Food
            {
                CarrierBee = BeeEntities[index]
            });
        }
    }
}

[BurstCompile]
partial struct BeeSystem : ISystem
{
    private long _tick;
    
    private EntityQuery _foodQuery;
    private EntityQuery _nestQuery;
    private EntityQuery _beeQuery;
    
    private ComponentLookup<LocalToWorldTransform> _transformComponentLookup;
    private ComponentLookup<Food> _foodComponentLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Build queries
        var foodQueryBuilder = new EntityQueryBuilder(Allocator.Temp);
        foodQueryBuilder.WithAll<UnmatchedFood, Food, LocalToWorldTransform>();
        _foodQuery = state.GetEntityQuery(foodQueryBuilder);

        var nestQueryBuilder = new EntityQueryBuilder(Allocator.Temp);
        nestQueryBuilder.WithAll<Faction, Area, LocalToWorldTransform>();
        _nestQuery = state.GetEntityQuery(nestQueryBuilder);
        
        var beeQueryBuilder = new EntityQueryBuilder(Allocator.Temp);
        beeQueryBuilder.WithAll<BeeProperties, LocalToWorldTransform, Faction>();
        _beeQuery = state.GetEntityQuery(beeQueryBuilder);

        _transformComponentLookup = state.GetComponentLookup<LocalToWorldTransform>();
        _foodComponentLookup = state.GetComponentLookup<Food>();

        _tick = 0;
        
        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Set faction filters on relevant queries
        var factionUsed = (int)(_tick % 2L + 1L);
        _beeQuery.ResetFilter();
        _beeQuery.SetSharedComponentFilter(new Faction
        {
            Value = factionUsed
        });
        
        _nestQuery.ResetFilter();
        _nestQuery.SetSharedComponentFilter(new Faction
        {
            Value = factionUsed
        });
        
        var config = SystemAPI.GetSingleton<BeeConfig>();

        // Prepare bee action selection job
        // Food structures
        var foodEntities = _foodQuery.ToEntityArray(Allocator.TempJob);
        var foodTransforms = _foodQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var foodProperties = _foodQuery.ToComponentDataArray<Food>(Allocator.TempJob);
        
        // Bee structures
        var beeProperties = _beeQuery.ToComponentDataArray<BeeProperties>(Allocator.TempJob);
        var beeTransforms = _beeQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var beeEntities =  _beeQuery.ToEntityArray(Allocator.TempJob);
        
        // Nests
        var nestTransforms = _nestQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var nestAreas = _nestQuery.ToComponentDataArray<Area>(Allocator.TempJob);
        var nestEntities = _nestQuery.ToEntityArray(Allocator.TempJob);
        
        // Command buffer
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        _foodComponentLookup.Update(ref state);
        // BeeAction job instantiate and schedule
        var beeActionProcessingJob = new BeeActionProcessingJob
        {
            FoodTransforms = foodTransforms,
            FoodEntities = foodEntities,
            FoodProperties = foodProperties,
            BeeProperties = beeProperties,
            BeeTransforms = beeTransforms,
            BeeEntities = beeEntities,
            NestTransform = nestTransforms,
            NestEntities = nestEntities,
            NestAreas = nestAreas,
            Faction = factionUsed,
            Random = new Random((uint)(_tick + 1)),
            FoodComponentLookup = _foodComponentLookup,
            ECB = ecb
        };
        state.Dependency = beeActionProcessingJob.Schedule(state.Dependency);
        state.Dependency.Complete();
        
        // [IJobEntity only?]
        // NOTE: The following is the default pattern, you don't need to state it: (happens at line 57)
        // state.Dependency = foodTargetingJob.ScheduleParallel(state.Dependency);
        
        // NOTE: Implicitly contains dependency line 57
        // [QUESTION]Combine dependencies + dispose once == better?
        // [QUESTION]Changed from IJobEntity to IJob. Effect on auto-dependency management?
        state.Dependency = foodTransforms.Dispose(state.Dependency);
        state.Dependency = foodEntities.Dispose(state.Dependency);
        state.Dependency = foodProperties.Dispose(state.Dependency);
        state.Dependency = beeProperties.Dispose(state.Dependency);
        state.Dependency = beeEntities.Dispose(state.Dependency);
        state.Dependency = beeTransforms.Dispose(state.Dependency);
        state.Dependency = nestTransforms.Dispose(state.Dependency);
        state.Dependency = nestAreas.Dispose(state.Dependency);
        state.Dependency = nestEntities.Dispose(state.Dependency);
        
        // Point towards target job
        _transformComponentLookup.Update(ref state);
        var beeTargetingJob = new BeePointToTargetJob // IJobEntity
        {
            ComponentLookup = _transformComponentLookup,
            TimeStep = state.Time.DeltaTime,
            GravityDt = config.gravity * state.Time.DeltaTime,
            BeeSpeed = config.beeSpeed,
            ObjectSize = config.objectSize,
            ECB = ecb.AsParallelWriter()
        };
        state.Dependency = beeTargetingJob.ScheduleParallel(state.Dependency);

        _tick++;
    }
}