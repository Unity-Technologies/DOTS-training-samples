using Packages.Rider.Editor.UnitTesting;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

/// <summary>
/// Assigns tasks to bees without one
/// </summary>
[UpdateAfter(typeof(TargetingValidationSystem))]
public class TargetAcquisitionSystem : SystemBase
{
    EntityQuery m_AvailableFoodQuery;
    EntityQuery m_Team1BeesQuery;
    EntityQuery m_Team2BeesQuery;
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
        m_Team1BeesQuery = GetEntityQuery(typeof(BeeTag), typeof(Team1));
        m_Team2BeesQuery = GetEntityQuery(typeof(BeeTag), typeof(Team2));
        m_Random = new Random(1234);
    }

    protected override void OnUpdate()
    {
        // Let's get all resources that are not currently carried
        var availableFood = m_AvailableFoodQuery.ToEntityArray(Allocator.Temp);
        var team1Bees = m_Team1BeesQuery.ToEntityArray(Allocator.Temp);
        var team2Bees = m_Team2BeesQuery.ToEntityArray(Allocator.Temp);
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = m_Random;
        if (availableFood.Length > 0 || team2Bees.Length > 0)
        {
            Entities
                .WithNone<CarriedFood, MoveTarget, TargetPosition>()
                .WithAll<BeeTag, Team1>()
                .ForEach((Entity e) =>
                {
                    AcquireTarget(e, ecb, random, availableFood, team2Bees);
                }).Run();
        }
        
        if (availableFood.Length > 0 || team1Bees.Length > 0)
        {
            Entities
                .WithNone<CarriedFood, MoveTarget, TargetPosition>()
                .WithAll<BeeTag, Team2>()
                .ForEach((Entity e) =>
                {
                    AcquireTarget(e, ecb, random, availableFood, team1Bees);
                }).Run();
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
        team1Bees.Dispose();
        team2Bees.Dispose();
        availableFood.Dispose();
    }

    static void AcquireTarget(Entity bee, EntityCommandBuffer ecb, Random random, NativeArray<Entity> availableFood, NativeArray<Entity> targetBees)
    {
        var targetIndex = random.NextInt(availableFood.Length + targetBees.Length);
        Entity target;
        if (targetIndex < availableFood.Length)
        {
            target = availableFood[targetIndex];
            ecb.AddComponent<FetchingFodTag>(bee);
        }
        else
        {
            target = targetBees[targetIndex - availableFood.Length];
            ecb.AddComponent<AttackingBeeTag>(bee);
        }
        
        ecb.AddComponent(bee, new MoveTarget
        {
            Value = target,
        });
        ecb.AddComponent<TargetPosition>(bee);
    }
}
