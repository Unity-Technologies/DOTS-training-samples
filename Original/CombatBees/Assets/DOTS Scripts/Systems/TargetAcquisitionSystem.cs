using Unity.Collections;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

/// <summary>
/// Assigns tasks to bees without one
/// </summary>
[UpdateAfter(typeof(TargetingValidationSystem))]
public class TargetAcquisitionSystem : SystemBase
{
    EntityQuery m_AvailableFoodQuery;
    Random m_Random;

    protected override void OnCreate()
    {
        var availableFoodQueryDescription = new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(FoodTag),
            },
            None = new ComponentType[]
            {
                typeof(CarrierBee),
                typeof(IntentionallyDroppedFoodTag)
            },
        };

        m_AvailableFoodQuery = GetEntityQuery(availableFoodQueryDescription);
        m_Random = new Random(1234);
    }

    protected override void OnUpdate()
    {
        // Let's get all resources that are not currently carried
        var availableFood = m_AvailableFoodQuery.ToEntityArray(Allocator.Temp);
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = m_Random;
        Entities
            .WithNone<CarriedFood, MoveTarget, TargetPosition>()
            .WithAll<BeeTag>()
            .WithDisposeOnCompletion(availableFood)
            .ForEach((Entity e) =>
            {
                if (availableFood.Length != 0)
                {
                    var targetIndex = random.NextInt(availableFood.Length);
                    var target = availableFood[targetIndex];
                    ecb.AddComponent(e, new MoveTarget
                    {
                        Value = target,
                    });
                    ecb.AddComponent<TargetPosition>(e);
                }

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
