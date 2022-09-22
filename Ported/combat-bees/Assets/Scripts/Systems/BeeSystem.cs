using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[WithNone(typeof(Dead))]
[BurstCompile]
partial struct BeePointToTargetJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalToWorldTransform> ComponentLookup;

    [ReadOnly] public float TimeStep;
    [ReadOnly] public float GravityDt;
    [ReadOnly] public float BeeSpeed;
    [ReadOnly] public float ObjectSize;

    public EntityCommandBuffer.ParallelWriter ECB;

    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int chunkIndex, in LocalToWorldTransform transform,
        ref BeeProperties beeProperties, ref Velocity velocity)
    {
        if (beeProperties.Target == Entity.Null)
            return;
        
        switch (beeProperties.BeeMode)
        {
            case BeeMode.Attack:
            case BeeMode.MoveResourceBackToNest:
            case BeeMode.HeadTowardsResource:
                // Get latest position of target
                var newPosition = ComponentLookup.GetRefRO(beeProperties.Target).ValueRO.Value.Position;
                // work a small offset into the target position to account for object sizes
                newPosition.y += ObjectSize;

                beeProperties.TargetPosition = newPosition;


                var vel = MovementUtilities.ComputeTargetVelocity(
                    transform.Value.Position, newPosition, GravityDt, TimeStep, BeeSpeed);
                velocity.Value = vel;

                if (beeProperties.BeeMode == BeeMode.MoveResourceBackToNest)
                {
                    ECB.SetComponent(chunkIndex, beeProperties.CarriedFood, new Velocity
                    {
                        Value = vel
                    });
                }

                break;
        }
    }
}

[WithNone(typeof(Dead))]
[BurstCompile]
partial struct BeeActionProcessingJob : IJobEntity
{
    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<LocalToWorldTransform> FoodTransforms;

    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<LocalToWorldTransform> Team2Transforms;

    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<LocalToWorldTransform> Team1Transforms;

    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> FoodEntities;

    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Food> FoodProperties;

    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> Team1Entities;

    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> Team2Entities;

    [DeallocateOnJobCompletion]
    [ReadOnly] public NativeArray<LocalToWorldTransform> Nest1Transform;

    [DeallocateOnJobCompletion]
    [ReadOnly] public NativeArray<Entity> Nest1Entities;

    [DeallocateOnJobCompletion]
    [ReadOnly] public NativeArray<Area> Nest1Areas;
    
    [DeallocateOnJobCompletion]
    [ReadOnly] public NativeArray<LocalToWorldTransform> Nest2Transform;

    [DeallocateOnJobCompletion]
    [ReadOnly] public NativeArray<Entity> Nest2Entities;

    [DeallocateOnJobCompletion]
    [ReadOnly] public NativeArray<Area> Nest2Areas;


    [ReadOnly] public uint Tick;

    [ReadOnly] public ComponentLookup<Food> FoodComponentLookup;

    public EntityCommandBuffer.ParallelWriter ECB;
    
    [BurstCompile]
    public void Execute([EntityInQueryIndex] int entityInQueryIndex, [ChunkIndexInQuery] int chunkIndex,
        Entity beeEntity, in LocalToWorldTransform transform, in Faction faction, ref BeeProperties beeProperties)
    {
        switch (beeProperties.BeeMode)
        {
            case BeeMode.FindAttackTarget:
                if (faction.Value == (int)Factions.Team1)
                {
                    FindTargetForBee(ref beeProperties, chunkIndex, entityInQueryIndex, Team2Entities,
                        BeeMode.Attack,
                        Team2Transforms);
                }
                else
                {
                    FindTargetForBee(ref beeProperties, chunkIndex, entityInQueryIndex, Team1Entities,
                        BeeMode.Attack, Team1Transforms);
                }

                break;
            case BeeMode.Idle:
                var random = Random.CreateFromIndex((uint)entityInQueryIndex + Tick);

                // Switch to Attack or FindResource
                beeProperties = new BeeProperties
                {
                    BeeMode = random.NextFloat() < beeProperties.Aggressivity
                        ? BeeMode.FindAttackTarget
                        : BeeMode.FindResource,
                    Target = Entity.Null,
                    Aggressivity = beeProperties.Aggressivity,
                    CarriedFood = Entity.Null,
                    TargetPosition = float3.zero
                };
                break;
            case BeeMode.FindResource:
                // Pick a target resource and switch to HeadTowardsResource mode
                FindTargetForBee(ref beeProperties, chunkIndex, entityInQueryIndex, FoodEntities,
                    BeeMode.HeadTowardsResource, FoodTransforms);
                break;
            case BeeMode.HeadTowardsResource:
                TryPickupTargetResource(ref beeProperties, transform, chunkIndex, beeEntity, faction);
                // Head towards targeted resource + pickup on reaching it
                break;
            case BeeMode.MoveResourceBackToNest:
                TryDropoffResource(chunkIndex, beeEntity, transform, beeProperties, faction);
                // Return to base with resource / No action
                break;
        }
    }

    private void FindTargetForBee(ref BeeProperties beeProperties, int chunkIndex, int entityInQueryIndex,
        NativeArray<Entity> targetEntities, BeeMode nextMode, NativeArray<LocalToWorldTransform> TargetTransforms)
    {
        if (targetEntities.Length == 0)
        {
            beeProperties = new BeeProperties
            {
                Aggressivity = beeProperties.Aggressivity,
                Target = Entity.Null,
                BeeMode = BeeMode.Idle,
                TargetPosition = float3.zero
            };
            return;
        }

        var random = Random.CreateFromIndex((uint)entityInQueryIndex + Tick);

        var index = random.NextInt(targetEntities.Length);

        beeProperties = new BeeProperties
        {
            Aggressivity = beeProperties.Aggressivity,
            Target = targetEntities[index],
            BeeMode = nextMode,
            TargetPosition = TargetTransforms[index].Value.Position
        };

        ECB.RemoveComponent<UnmatchedFood>(chunkIndex, targetEntities[index]);
    }

    private void TryDropoffResource(int chunkIndex, Entity beeEntity, LocalToWorldTransform transform,
        BeeProperties beeProperties, Faction faction)
    {
        NativeArray<Area> NestAreas = faction.Value == (int)Factions.Team1 ? Nest1Areas : Nest2Areas;
        // Check if in bounds of nest
        if (!NestAreas[0].Value.Contains(transform.Value.Position))
        {
            return;
        }

        ECB.SetComponent(chunkIndex, beeProperties.CarriedFood, new Velocity
        {
            Value = float3.zero
        });

        ECB.SetComponent(chunkIndex, beeProperties.CarriedFood, new Food
        {
            CarrierBee = Entity.Null
        });
        ECB.SetSharedComponent(chunkIndex, beeProperties.CarriedFood, new Faction
        {
            Value = faction.Value
        });

        ECB.SetComponent(chunkIndex, beeEntity, new BeeProperties
        {
            BeeMode = BeeMode.Idle,
            Target = Entity.Null,
            CarriedFood = Entity.Null,
            Aggressivity = beeProperties.Aggressivity,
            TargetPosition = float3.zero
        });
    }

    private void TryPickupTargetResource(ref BeeProperties beeProperties, in LocalToWorldTransform beeTransform,
        int chunkIndex, Entity beeEntity, Faction faction)
    {
        if (!FoodComponentLookup.HasComponent(beeProperties.Target))
        {
            beeProperties = new BeeProperties
            {
                BeeMode = BeeMode.Idle,
                Aggressivity = beeProperties.Aggressivity,
                Target = Entity.Null,
                TargetPosition = float3.zero,
                CarriedFood = Entity.Null
            };
            return;
        }
            
        var foodCarrier = FoodComponentLookup.GetRefRO(beeProperties.Target).ValueRO.CarrierBee;

        NativeArray<Entity> NestEntities = faction.Value == (int)Factions.Team1 ? Nest1Entities : Nest2Entities;
        NativeArray<LocalToWorldTransform> NestTransform = faction.Value == (int)Factions.Team1 ? Nest1Transform : Nest2Transform;
            
        if (foodCarrier != Entity.Null)
        {
            beeProperties = new BeeProperties
            {
                BeeMode = BeeMode.Idle,
                Aggressivity = beeProperties.Aggressivity,
                Target = Entity.Null,
                TargetPosition = float3.zero,
                CarriedFood = Entity.Null
            };
            return;
        }
        
        // Check distance to target, if within a certain range, pickup the food.
        var deltaPosition = beeProperties.TargetPosition - beeTransform.Value.Position;
        if (math.lengthsq(deltaPosition) < 0.01f)
        {
            beeProperties = new BeeProperties
            {
                BeeMode = BeeMode.MoveResourceBackToNest,
                Aggressivity = beeProperties.Aggressivity,
                Target = NestEntities[0],
                TargetPosition = NestTransform[0].Value.Position,
                CarriedFood = beeProperties.Target
            };

            if (FoodComponentLookup.HasComponent(beeProperties.Target))
            {
                ECB.SetComponent(chunkIndex, beeProperties.Target, new Food
                {
                    CarrierBee = beeEntity,
                });
            }
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
        foodQueryBuilder.WithAll<UnmatchedFood, Food, LocalToWorldTransform, Faction>();
        _foodQuery = state.GetEntityQuery(foodQueryBuilder);
        _foodQuery.SetSharedComponentFilter(new Faction
        {
            Value = 0
        });
        var nestQueryBuilder = new EntityQueryBuilder(Allocator.Temp);
        nestQueryBuilder.WithAll<Faction, Area, LocalToWorldTransform>();
        _nestQuery = state.GetEntityQuery(nestQueryBuilder);

        _beeQuery = SystemAPI.QueryBuilder().WithAll<BeeProperties, LocalToWorldTransform, Faction>().Build();

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
        var config = SystemAPI.GetSingleton<BeeConfig>();

        _foodComponentLookup.Update(ref state);
        _transformComponentLookup.Update(ref state);
        
        // Prepare bee action selection job
        // Food structures
        var foodEntities = _foodQuery.ToEntityArray(Allocator.TempJob);
        var foodTransforms = _foodQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var foodProperties = _foodQuery.ToComponentDataArray<Food>(Allocator.TempJob);

        _beeQuery.SetSharedComponentFilter(new Faction { Value = (int)Factions.Team1 });
        var team1Bees = _beeQuery.ToEntityArray(Allocator.TempJob);
        var team1Transforms = _beeQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);

        _beeQuery.SetSharedComponentFilter(new Faction { Value = (int)Factions.Team2 });
        var team2Bees = _beeQuery.ToEntityArray(Allocator.TempJob);
        var team2Transforms = _beeQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);

        _foodComponentLookup.Update(ref state);

        _nestQuery.SetSharedComponentFilter(new Faction
        {
            Value = (int)Factions.Team1
        });
        
        var nest1Transforms = _nestQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var nest1Areas = _nestQuery.ToComponentDataArray<Area>(Allocator.TempJob);
        var nest1Entities = _nestQuery.ToEntityArray(Allocator.TempJob);

        _nestQuery.SetSharedComponentFilter(new Faction
        {
            Value = (int)Factions.Team2
        });
        
        var nest2Transforms = _nestQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var nest2Areas = _nestQuery.ToComponentDataArray<Area>(Allocator.TempJob);
        var nest2Entities = _nestQuery.ToEntityArray(Allocator.TempJob);

        // Command buffer
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        // BeeAction job instantiate and schedule
        var beeActionProcessingJob = new BeeActionProcessingJob
        {
            FoodTransforms = foodTransforms,
            Team1Transforms = team1Transforms,
            Team2Transforms = team2Transforms,
            FoodEntities = foodEntities,
            FoodProperties = foodProperties,
            Team1Entities = team1Bees,
            Team2Entities = team2Bees,
            Nest1Transform = nest1Transforms,
            Nest1Entities = nest1Entities,
            Nest1Areas = nest1Areas,
            
            Nest2Transform = nest2Transforms,
            Nest2Entities = nest2Entities,
            Nest2Areas = nest2Areas,
            Tick = (uint)_tick,
            FoodComponentLookup = _foodComponentLookup,
            ECB = ecb.AsParallelWriter(),
        };
        beeActionProcessingJob.ScheduleParallel(state.Dependency).Complete();

        // Point towards target job
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