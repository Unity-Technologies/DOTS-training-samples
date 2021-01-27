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
    EntityQuery m_AvailableResourcesQuery;
    Random m_Random;
    NativeArray<Entity> m_AvailableFood;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        var availableResourcesQueryDescription = new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(FoodTag),
            },
            None = new ComponentType[]
            {
                typeof(CarriedResource),
            },
        };

        m_AvailableResourcesQuery = GetEntityQuery(availableResourcesQueryDescription);
        m_Random = new Random(1234);
    }

    protected override void OnUpdate()
    {
        if (m_AvailableFood.IsCreated)
        {
            m_AvailableFood.Dispose();
        }
        // Let's get all resources that are not currently carried
        m_AvailableFood = m_AvailableResourcesQuery.ToEntityArray(Allocator.TempJob);
        var availableFood = m_AvailableFood;
        
        var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();
        var random = m_Random;
        Entities
            .WithNone<CarriedResource, MoveTarget, TargetPosition>()
            .WithAll<BeeTag>()
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

    protected override void OnDestroy()
    {
        if (m_AvailableFood.IsCreated)
        {
            m_AvailableFood.Dispose();
        }
    }
}
