using Unity.Collections;
using Unity.Entities;
public class DropFoodSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        int dropRatePerFrame = 1;


        NativeArray<Entity> newlyDroppedFoodResources = state.EntityManager.Instantiate(config.FoodResourcePrefab, dropRatePerFrame, Allocator.Temp);
        Random rand = new Random(123);
        //foreach(var foodResource in newlyDroppedFoodResources)
        //{
        //    foodResource.Index
        //    state.EntityManager.
        //}
        foreach (var (foodResource, transform) in SystemAPI.Query<RefRW<FoodResource>, TransformAspect>().WithAll<FoodResource>())
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
