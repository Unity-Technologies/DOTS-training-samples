using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

public class TargetSelectionSystem : SystemBase
{
    private EntityQuery m_AvailableCansQuery;

    protected override void OnCreate()
    {
        m_AvailableCansQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Can>(),
            ComponentType.ReadOnly<Available>());
    }

    protected override void OnUpdate()
    {
        var availableCans = m_AvailableCansQuery.ToEntityArray(Allocator.TempJob);
        var translations = GetComponentDataFromEntity<Translation>();
        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = sys.CreateCommandBuffer();
        var ecbParaWriter = ecb.AsParallelWriter();

        Entities
            .WithAll<HandLookingForACan>()
            .WithReadOnly(translations)
            .WithReadOnly(availableCans)
            .WithDisposeOnCompletion(availableCans)
            .ForEach((Entity entity, int entityInQueryIndex, ref TargetCan targetCan, in Translation translation) =>
            {
                if (availableCans.Length > 0)
                {
                    if (Utils.FindNearestCan(translation, availableCans, translations, out Entity nearestCan))
                    {
                        Utils.GoToState<HandLookingForACan, HandWindingUp>(ecbParaWriter, entityInQueryIndex, entity);
                        
                        targetCan.Value = nearestCan;
                        ecbParaWriter.RemoveComponent<Available>(entityInQueryIndex, nearestCan);
                    }
                    else
                    {
                        targetCan.Value = Entity.Null;
                    }
                }
            }).ScheduleParallel();

        sys.AddJobHandleForProducer(Dependency);
    }
}
