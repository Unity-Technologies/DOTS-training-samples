using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
partial struct DropFoodSystem : ISystem
{
    Random rand;
    float timeSinceLastDrop;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        rand = new Random(123);
        timeSinceLastDrop = 0.0f;
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        timeSinceLastDrop += state.Time.DeltaTime;

        if ((1.0f / config.FoodResourceDropRatePerSecond) <= timeSinceLastDrop)
        {
            state.EntityManager.Instantiate(config.FoodResourcePrefab, 1, Allocator.Temp);
            timeSinceLastDrop = 0.0f;
        }

        // Component type FoodResource is implicitly defined by the query so no WithAll is needed.
        // WithAll puts ADDITIONAL requirements on the query.
        foreach (var (foodResource, transform) in SystemAPI.Query<RefRW<FoodResource>, TransformAspect>())
        {
            switch (foodResource.ValueRW.State)
            {
                case FoodState.NULL:
                    RandomlyPlaceDroppedFoodInPlayArea(ref state, foodResource, transform);
                    break;
                case FoodState.FALLING:
                    UpdatePositionOfFallingFood(ref state, ref config, foodResource, transform);
                    break;
            }
        }
    }

    public void RandomlyPlaceDroppedFoodInPlayArea(ref SystemState state, RefRW<FoodResource> foodResource, TransformAspect transform)
    {
        var position = new float3(rand.NextInt(-40, 41), rand.NextInt(7, 13), rand.NextInt(-15, 16));
        transform.Position = position;

        foodResource.ValueRW.State = FoodState.FALLING;
    }

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
}
