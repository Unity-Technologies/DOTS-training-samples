using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
partial struct DropFoodSystem : ISystem
{
    Random rand;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        rand = new Random(123);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        int dropRatePerFrame = 1;

        var config = SystemAPI.GetSingleton<Config>();

        NativeArray<Entity> newlyDroppedFoodResources = state.EntityManager.Instantiate(config.FoodResourcePrefab, dropRatePerFrame, Allocator.Temp);
        // have default tag on newly generated entities
        foreach (var (foodResource, transform) in SystemAPI.Query<RefRW<FoodResource>, TransformAspect>())
        // Component type FoodResource is implicitly defined by the query so no WithAll is needed.
        // WithAll puts ADDITIONAL requirements on the query.
        {
            if (foodResource.ValueRW.State == FoodState.NULL)
            {
                var position = new float3(rand.NextInt(-40, 41), rand.NextInt(7, 13), rand.NextInt(-15, 16));
                transform.Position = position;

                foodResource.ValueRW.State = FoodState.FALLING;
            }
        }
    }
}
