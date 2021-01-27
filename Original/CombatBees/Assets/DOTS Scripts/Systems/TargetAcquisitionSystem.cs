using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

/// <summary>
/// Assigns tasks to bees without one
/// </summary>
public class TargetAcquisitionSystem : SystemBase
{
    EntityCommandBufferSystem m_EntityCommandBufferSystem;
    EntityQuery m_AvailableFoodQuery;
    Random m_Random;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        var availableFoodQueryDescription = new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(FoodTag),
            },
            None = new ComponentType[]
            {
                typeof(CarrierBee),
            },
        };

        m_AvailableFoodQuery = GetEntityQuery(availableFoodQueryDescription);
        m_Random = new Random(1234);
    }

    protected override void OnUpdate()
    {
        // Let's get all resources that are not currently carried
        var availableFood = m_AvailableFoodQuery.ToEntityArray(Allocator.TempJob);
        
        var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();
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

            }).Schedule();
        
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
