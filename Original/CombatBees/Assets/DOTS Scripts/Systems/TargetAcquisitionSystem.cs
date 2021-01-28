using Packages.Rider.Editor.UnitTesting;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
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
    private EntityCommandBufferSystem ecbs;

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
        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Let's get all resources that are not currently carried
        var availableFood = m_AvailableFoodQuery.ToEntityArray(Allocator.TempJob);
        var team1Bees = m_Team1BeesQuery.ToEntityArray(Allocator.TempJob);
        var team2Bees = m_Team2BeesQuery.ToEntityArray(Allocator.TempJob);

        if (availableFood.Length > 0 || team2Bees.Length > 0)
        {
            var pecb = ecbs.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithNone<CarriedFood, MoveTarget, TargetPosition>()
                .WithNone<FetchingFoodTag, AttackingBeeTag>()
                .WithAll<BeeTag, Team1>()
                .WithReadOnly(availableFood)
                .WithReadOnly(team2Bees)
                .ForEach((Entity e, int entityInQueryIndex, ref RandomComponent random) =>
                {
                    AcquireTarget(e, entityInQueryIndex, pecb, ref random.Value, availableFood, team2Bees);
                }).ScheduleParallel();
        }

        if (availableFood.Length > 0 || team1Bees.Length > 0)
        {
            var pecb = ecbs.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithNone<CarriedFood, MoveTarget, TargetPosition>()
                .WithNone<FetchingFoodTag, AttackingBeeTag>()
                .WithAll<BeeTag, Team2>()
                .WithReadOnly(availableFood)
                .WithReadOnly(team1Bees)
                .ForEach((Entity e, int entityInQueryIndex, ref RandomComponent random) =>
                {
                    AcquireTarget(e, entityInQueryIndex, pecb, ref random.Value, availableFood, team1Bees);
                }).ScheduleParallel();
        }

        team1Bees.Dispose(Dependency);
        team2Bees.Dispose(Dependency);
        availableFood.Dispose(Dependency);

        ecbs.AddJobHandleForProducer(Dependency);
    }

    static void AcquireTarget(Entity bee, int entityInQueryIndex, EntityCommandBuffer.ParallelWriter ecb, ref Random random, NativeArray<Entity> availableFood, NativeArray<Entity> targetBees)
    {
        var targetIndex = random.NextInt(availableFood.Length + targetBees.Length);
        Entity target;
        ComponentTypes types;
        if (targetIndex < availableFood.Length)
        {
            types = new ComponentTypes(ComponentType.ReadOnly<MoveTarget>(), ComponentType.ReadOnly<TargetPosition>(), ComponentType.ReadOnly<FetchingFoodTag>());
            target = availableFood[targetIndex];
        }
        else
        {
            types = new ComponentTypes(ComponentType.ReadOnly<MoveTarget>(), ComponentType.ReadOnly<TargetPosition>(), ComponentType.ReadOnly<AttackingBeeTag>());
            target = targetBees[targetIndex - availableFood.Length];
        }
        
        ecb.AddComponent(entityInQueryIndex, bee, types);

        ecb.SetComponent(entityInQueryIndex, bee, new MoveTarget
        {
            Value = target,
        });
    }
}