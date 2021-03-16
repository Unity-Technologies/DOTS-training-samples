using System.Transactions;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public static class RandomExtensions
{
    public static float3 NextCarouselPosition(ref this Random random,
        WorldBounds worldBounds, float Depth = 0.0f, float MinHeight = 0.0f, float MaxHeight = 0.0f)
    {
        return new float3(
            random.NextFloat(0.0f, worldBounds.Width),
            random.NextFloat(MinHeight, MaxHeight),
            Depth);
    }
}


public class CarouselRecyclerSystem : SystemBase
{
    static Random random = new Random(1234);
    
    protected override void OnUpdate()
    {
        var worldBounds = GetSingleton<WorldBounds>();

        Entities
            .WithAll<Rock>()
            .ForEach((Entity entity, ref Translation translation) =>
            {
                if (worldBounds.IsOutOfBounds(translation.Value))
                {
                    translation.Value = random.NextCarouselPosition(worldBounds, 5.0f);
                }
            }).Run(); 
        
        Entities
            .WithAll<Can>()
            .ForEach((Entity entity, ref Translation translation) =>
            {
                if (worldBounds.IsOutOfBounds(translation.Value))
                {
                    translation.Value = random.NextCarouselPosition(worldBounds, 60.0f, 10.0f, 20.0f);
                }
            }).Run(); 
    }
}