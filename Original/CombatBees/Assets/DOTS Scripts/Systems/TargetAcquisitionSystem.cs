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

    protected override void OnCreate()
    {
        var availableFoodQueryDescription = new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<FoodTag>(),
            },
            None = new ComponentType[]
            {
                typeof(CarrierBee),
                typeof(IntentionallyDroppedFoodTag)
            },
        };

        m_AvailableFoodQuery = GetEntityQuery(availableFoodQueryDescription);
        m_Team1BeesQuery = GetEntityQuery(ComponentType.ReadOnly<BeeTag>(), ComponentType.ReadOnly<Team1>());
        m_Team2BeesQuery = GetEntityQuery(ComponentType.ReadOnly<BeeTag>(), ComponentType.ReadOnly<Team2>());
    }

    protected override void OnUpdate()
    {
        // Let's get all resources that are not currently carried
        var availableFood = m_AvailableFoodQuery.ToEntityArray(Allocator.Temp);
        var team1Bees = m_Team1BeesQuery.ToEntityArray(Allocator.Temp);
        var team2Bees = m_Team2BeesQuery.ToEntityArray(Allocator.Temp);
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        if (availableFood.Length > 0 || team2Bees.Length > 0)
        {
            Entities
                .WithNone<CarriedFood, MoveTarget, TargetPosition>()
                .WithNone<FetchingFoodTag, AttackingBeeTag>()
                .WithAll<BeeTag, Team1>()
                .ForEach((Entity e, ref RandomComponent random) =>
                {
                    AcquireTarget(e, ecb, ref random.Value, availableFood, team2Bees);
                }).Run();
        }
        
        if (availableFood.Length > 0 || team1Bees.Length > 0)
        {
            Entities
                .WithNone<CarriedFood, MoveTarget, TargetPosition>()
                .WithNone<FetchingFoodTag, AttackingBeeTag>()
                .WithAll<BeeTag, Team2>()
                .ForEach((Entity e, ref RandomComponent random) =>
                {
                    AcquireTarget(e, ecb, ref random.Value, availableFood, team1Bees);
                }).Run();
        }

        team1Bees.Dispose();
        team2Bees.Dispose();
        availableFood.Dispose();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static void AcquireTarget(Entity bee, EntityCommandBuffer ecb, ref Random random, NativeArray<Entity> availableFood, NativeArray<Entity> targetBees)
    {
        var targetIndex = random.NextInt(availableFood.Length + targetBees.Length);
        Entity target;
        if (targetIndex < availableFood.Length)
        {
            target = availableFood[targetIndex];
            ecb.AddComponent<FetchingFoodTag>(bee);
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
