using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

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
                    ECB.AddComponent(BeeEntities[i], new BeeProperties
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
            
        var deltaPosition = FoodTransforms[foodIndex].Value.Position -
                            BeeTransforms[i].Value.Position;
        var velocity = deltaPosition/math.sqrt(
            deltaPosition.x * deltaPosition.x + 
            deltaPosition.y * deltaPosition.y + 
            deltaPosition.z * deltaPosition.z);
            
        // Update bee velocity and target
        ECB.AddComponent(BeeEntities[i], new Velocity
        {
            Value = velocity
        });
        ECB.AddComponent(BeeEntities[i], new BeeProperties
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
        if (math.length(deltaPosition) < 0.1f)
        {
            ECB.AddComponent(BeeEntities[i], new BeeProperties
            {
                BeeMode = BeeMode.MoveResourceBackToNest,
                Aggressivity = BeeProperties[i].Aggressivity,
                Target = BeeProperties[i].Target,
                TargetPosition = BeeTransforms[i].Value.Position
            });
            ECB.AddComponent(BeeProperties[i].Target, new Food
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
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Food structures
        var foodEntities = _foodQuery.ToEntityArray(Allocator.TempJob);
        if (foodEntities.Length == 0)
        {
            foodEntities.Dispose();
            return;
        }
        var foodTransforms = _foodQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        
        // Bee structures
        var beeProperties = _beeQuery.ToComponentDataArray<BeeProperties>(Allocator.TempJob);
        var beeTransforms = _beeQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var beeEntities = _beeQuery.ToEntityArray(Allocator.TempJob);
        
        // Command buffer
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        // BeeAction job
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
    }
}