using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

partial struct FoodTargetingJob : IJob
{
    [ReadOnly]
    public NativeArray<LocalToWorldTransform> FoodTransforms;

    [ReadOnly]
    public NativeArray<Entity> FoodEntities;

    [ReadOnly]
    public float TimeElapsed;

    [ReadOnly]
    public NativeArray<LocalToWorldTransform> BeeTransforms;
    
    [ReadOnly]
    public NativeArray<BeeProperties> BeeProperties;
    
    [ReadOnly]
    public NativeArray<Entity> BeeEntities;

    public EntityCommandBuffer ECB;

    // Original parallel -> all bees in parallel, match to random food.
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
            // No more food: stop
            if (i >= FoodEntities.Length)
            {
                return;
            }
            // Bee has target: skip bee
            if (BeeProperties[i].Target != Entity.Null)
            {
                continue;
            }
            // Inverting index for food (temp, all bees spawn on food with match index)
            var foodIndex = FoodEntities.Length - (1 + i);
            
            var deltaPosition = BeeTransforms[i].Value.Position -
                                FoodTransforms[foodIndex].Value.Position;
            var velocity = deltaPosition/math.sqrt(
                deltaPosition.x * deltaPosition.x + 
                deltaPosition.y * deltaPosition.y + 
                deltaPosition.z * deltaPosition.z);
            
            // Update bee velocity and target
            ECB.AddComponent(BeeEntities[i], new Velocity
            {
                velocity = velocity
            });
            ECB.AddComponent(BeeEntities[i], new BeeProperties
            {
                Aggressivity = BeeProperties[i].Aggressivity,
                Target = FoodEntities[foodIndex],
                BeeMode = BeeProperties[i].BeeMode
            });
            
            // Update food's carrier bee. Remove UnmatchedFood tag
            ECB.AddComponent(FoodEntities[foodIndex], new Food
            {
                CarrierBee = BeeEntities[i]
            });
            ECB.RemoveComponent<UnmatchedFood>(FoodEntities[foodIndex]);
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
        var foodEntities = _foodQuery.ToEntityArray(Allocator.TempJob);
        if (foodEntities.Length == 0)
        {
            foodEntities.Dispose();
            return;
        }
        var foodTransforms = _foodQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        
        var beeProperties = _beeQuery.ToComponentDataArray<BeeProperties>(Allocator.TempJob);
        var beeTransforms = _beeQuery.ToComponentDataArray<LocalToWorldTransform>(Allocator.TempJob);
        var beeEntities = _beeQuery.ToEntityArray(Allocator.TempJob);
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var foodTargetingJob = new FoodTargetingJob
        {
            TimeElapsed = (float)(state.Time.ElapsedTime * 100000d),
            FoodTransforms = foodTransforms,
            FoodEntities = foodEntities,
            BeeProperties = beeProperties,
            BeeTransforms = beeTransforms,
            BeeEntities = beeEntities,
            ECB = ecb
        };
        state.Dependency = foodTargetingJob.Schedule(state.Dependency);
        // [IJobEntity only?]
        // NOTE: The following is the default pattern, you don't need to state it: (happens at line 57)
        // state.Dependency = foodTargetingJob.ScheduleParallel(state.Dependency);
        
        // NOTE: Implicitly contains dependency line 57
        // Probably dont need that assignment since no other job will risk using this array
        // [QUESTION]Combine dependencies + dispose once == better?
        // [QUESTION]Changed from IJobEntity to IJob. Effect on auto-dependency management?
        // [QUESTION]IJob, done in this way, schedules once per frame? Noticed the system had 100+ entities in Editor,
        // what does that mean? Seems like it behaves as expected, one run per update.
        state.Dependency = foodTransforms.Dispose(state.Dependency);
        state.Dependency = foodEntities.Dispose(state.Dependency);
        state.Dependency = beeProperties.Dispose(state.Dependency);
        state.Dependency = beeEntities.Dispose(state.Dependency);
        state.Dependency = beeTransforms.Dispose(state.Dependency);
    }
}