using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[WithNone(typeof(Dead))]
[BurstCompile]
partial struct BeePointToTargetJob : IJobEntity
{
    [ReadOnly]
    public ComponentLookup<LocalToWorldTransform> ComponentLookup;

    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int chunkIndex, in LocalToWorldTransform transform,
        ref BeeProperties beeProperties, ref Velocity velocity)
    {

        switch (beeProperties.BeeMode)
        {
            case BeeMode.Attack:
            case BeeMode.HeadTowardsResource:
            // Get latest position of target
                var newPosition = ComponentLookup.GetRefRO(beeProperties.Target).ValueRO.Value.Position;
                beeProperties.TargetPosition = newPosition;

                // Compute new velocity
                var deltaPosition = newPosition - transform.Value.Position;
                velocity.Value = math.normalizesafe(deltaPosition) * 10.0f;
            break;
        }

    }
}

[WithNone(typeof(Dead))]
[BurstCompile]
partial struct BeeActionProcessingJob : IJobEntity
{
    [DeallocateOnJobCompletion]
    [ReadOnly]
    public NativeArray<LocalToWorldTransform> FoodTransforms;
    
    [DeallocateOnJobCompletion]
    [ReadOnly]
    public NativeArray<LocalToWorldTransform> Team2Transforms;
    
    [DeallocateOnJobCompletion]
    [ReadOnly]
    public NativeArray<LocalToWorldTransform> Team1Transforms;

    [DeallocateOnJobCompletion]
    [ReadOnly]
    public NativeArray<Entity> FoodEntities;

    [DeallocateOnJobCompletion]
    [ReadOnly]
    public NativeArray<Entity> Team1Entities;

    [DeallocateOnJobCompletion]
    [ReadOnly]
    public NativeArray<Entity> Team2Entities;
    
    [ReadOnly] public ComponentLookup<Food> Foods;

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
                    FindTargetForBee(ref beeProperties, chunkIndex, entityInQueryIndex, Team2Entities, BeeMode.Attack,
                        Team2Transforms);
                }
                else
                {
                    FindTargetForBee(ref beeProperties, chunkIndex, entityInQueryIndex, Team1Entities, BeeMode.Attack, Team1Transforms);
                }
                break;
            case BeeMode.Idle:
                var random = Random.CreateFromIndex((uint)entityInQueryIndex); 
                
                // Switch to Attack or FindResource
                beeProperties = new BeeProperties
                {
                    BeeMode = random.NextFloat() > beeProperties.Aggressivity ? BeeMode.FindResource : BeeMode.FindAttackTarget,
                    Aggressivity = beeProperties.Aggressivity,
                    Target = Entity.Null
                };
                break;
            case BeeMode.FindResource:
                // Pick a target resource and switch to HeadTowardsResource mode
                FindTargetForBee(ref beeProperties, chunkIndex, entityInQueryIndex, FoodEntities, BeeMode.HeadTowardsResource, FoodTransforms);
                break;
            case BeeMode.HeadTowardsResource:
                TryPickupTargetResource(ref beeProperties, transform, chunkIndex, beeEntity);
                // Head towards targeted resource + pickup on reaching it
                break;
            case BeeMode.MoveResourceBackToNest:
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
        
        var random = Random.CreateFromIndex((uint)entityInQueryIndex);
        
        // Inverting index for food (temp hack, all bees spawn on food with match index)
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

    private void TryPickupTargetResource(ref BeeProperties beeProperties, in LocalToWorldTransform beeTransform, int chunkIndex, Entity beeEntity)
    {
        // Check distance to target, if within a certain range, pickup the food.
        var deltaPosition = beeProperties.TargetPosition - beeTransform.Value.Position;
        if (math.length(deltaPosition) < 0.1f)
        {
            beeProperties = new BeeProperties
            {
                BeeMode = BeeMode.MoveResourceBackToNest,
                Aggressivity = beeProperties.Aggressivity,
                Target = beeProperties.Target,
                TargetPosition = beeTransform.Value.Position
            };
            
            if (Foods.HasComponent(beeProperties.Target))
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
    private EntityQuery _foodQuery;
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
        _beeQuery = SystemAPI.QueryBuilder().WithAll<BeeProperties, Faction, LocalToWorldTransform>().Build();
        
        _transformComponentLookup = state.GetComponentLookup<LocalToWorldTransform>();
        _foodComponentLookup = state.GetComponentLookup<Food>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // TODO further breakup into separate jobs and only run the necessary ones. Before there was a check for valid food at this point which skipped job if none were found. Same concept, just better.
        
        // Prepare bee action selection job
        // Food structures
        var foodEntities = _foodQuery.ToEntityArray(Allocator.TempJob);
        var foodTransforms = _foodQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);

        _beeQuery.SetSharedComponentFilter(new Faction { Value = (int)Factions.Team1 });
        var team1Bees = _beeQuery.ToEntityArray(Allocator.TempJob);
        var team1Transforms = _beeQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);

        _beeQuery.SetSharedComponentFilter(new Faction { Value = (int)Factions.Team2 });
        var team2Bees = _beeQuery.ToEntityArray(Allocator.TempJob);
        var team2Transforms = _beeQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        
        _foodComponentLookup.Update(ref state);
        
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
            Team1Entities = team1Bees,
            Team2Entities = team2Bees,
            ECB = ecb.AsParallelWriter(),
            Foods = _foodComponentLookup,
        };
        beeActionProcessingJob.ScheduleParallel();
 
        // Point towards target job
        _transformComponentLookup.Update(ref state);
        var beeTargetingJob = new BeePointToTargetJob // IJobEntity
        {
            ComponentLookup = _transformComponentLookup
        };
        beeTargetingJob.ScheduleParallel();
    }
}