using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct DropFoodSystem : ISystem
{
    Random rand;
    float timeSinceLastDrop;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        rand = new Random();
        timeSinceLastDrop = 0.0f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        if (rand.state == 0)
        {
            rand.InitState(config.RandomNumberSeed);
        }

        if (config.FoodResourceDropRatePerSecond != 0)
        {
            DropFoodInPlayArea(ref state, ref config);
        }

        // Create an EntityCommandBuffer.
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        // Component type FoodResource is implicitly defined by the query so no WithAll is needed.
        // WithAll puts ADDITIONAL requirements on the query.
        foreach (var (entity, foodResource, transform) in SystemAPI.Query<Entity, RefRW<FoodResource>, TransformAspect>())
        {
            switch (foodResource.ValueRW.State)
            {
                case FoodState.NULL:
                    RandomlyPlaceDroppedFoodInPlayArea(ref state, foodResource, transform);
                    break;
                case FoodState.FALLING:
                    UpdatePositionOfFallingFood(ref state, ref config, foodResource, transform);
                    break;
                case FoodState.SETTLED:
                    MarkFoodSettleInHiveAsDelivered(ref state, foodResource, transform);
                    break;
                case FoodState.DELIVERED:
                    SpawnNewBeesWhereFoodIsDelivered(ref state, ref config, foodResource, transform);
                    ecb.DestroyEntity(entity);
                    break;
            }
        }
        // Playback and dispose the EntityCommandBuffer.
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    public void DropFoodInPlayArea(ref SystemState state, ref Config config)
    {
        timeSinceLastDrop += state.Time.DeltaTime;

        if (config.FoodResourceDropRatePerSecond != 0 && ((1.0f / config.FoodResourceDropRatePerSecond) <= timeSinceLastDrop))
        {
            state.EntityManager.Instantiate(config.FoodResourcePrefab, 1, Allocator.Temp);
            timeSinceLastDrop = 0.0f;
        }
    }

    [BurstCompile]
    public void RandomlyPlaceDroppedFoodInPlayArea(ref SystemState state, RefRW<FoodResource> foodResource, TransformAspect transform)
    {
        var position = new float3(rand.NextInt(-40, 41), rand.NextInt(7, 13), rand.NextInt(-15, 16));
        transform.Position = position;

        foodResource.ValueRW.State = FoodState.FALLING;
    }

    //[BurstCompile]
    public void UpdatePositionOfFallingFood(ref SystemState state, ref Config config, RefRW<FoodResource> foodResource, TransformAspect transform)
    {
        var position = new float3(transform.Position.x, transform.Position.y - config.FallingSpeed, transform.Position.z);
        if (position.y <= 0.0f)
        {
            position.y = 0.0f;
            foodResource.ValueRW.State = FoodState.SETTLED;
        }
        transform.Position = position;
    }

    [BurstCompile]
    public void MarkFoodSettleInHiveAsDelivered(ref SystemState state, RefRW<FoodResource> foodResource, TransformAspect transform)
    {
        // if food position is in either hive (x and z)
        if (((-50 <= transform.Position.x && transform.Position.x <= -40) && (-15 <= transform.Position.z && transform.Position.z <= 15)) ||
            ((40 <= transform.Position.x && transform.Position.x <= 50) && (-15 <= transform.Position.z && transform.Position.z <= 15)))
        {
            // mark it as delivered
            foodResource.ValueRW.State = FoodState.DELIVERED;
        }
    }

    //[BurstCompile]
    public void SpawnNewBeesWhereFoodIsDelivered(ref SystemState state, ref Config config, RefRW<FoodResource> foodResource, TransformAspect foodTransform)
    {
        // check which hive the food is in
        Team beeTeam = Team.BLUE;
        Entity beePrefab = config.BlueBeePrefab;
        if (40 <= foodTransform.Position.x && foodTransform.Position.x <= 50)
        {
            beeTeam = Team.YELLOW;
            beePrefab = config.YellowBeePrefab;
        }

        // spawn bees similar to spawner system
        NativeArray<Entity> spawnedBees = state.EntityManager.Instantiate(beePrefab, rand.NextInt(config.MinNumberOfBeesSpawned, config.MaxNumberOfBeesSpawned), Allocator.Temp);

        // add tag for newly spawned stuff by default and remove it after initialization

        foreach (Entity beeEntity in spawnedBees)
        {
            // spawn bee at position where food has been dropped
            //var beeTransform = SystemAPI.GetComponent<TransformAspect>(beeEntity);
            var beeTransform = state.EntityManager.GetAspect<TransformAspect>(beeEntity);
            beeTransform.Position = foodTransform.Position;

            // make team color match hive
            //var beeComponent = SystemAPI.GetComponent<Bee>(beeEntity);
            //var beeComponent = state.EntityManager.GetComponent<Bee>(beeEntity);
            var beeComponent = state.EntityManager.GetComponentData<Bee>(beeEntity);
            beeComponent.SpawnPoint = foodTransform.Position;
            beeComponent.beeTeam = beeTeam;
            //SystemAPI.SetComponent<Bee>(beeEntity, beeComponent);
            //state.EntityManager.SetComponent<Bee>(beeEntity, beeComponent);
            state.EntityManager.SetComponentData<Bee>(beeEntity, beeComponent);
        }
    }
}