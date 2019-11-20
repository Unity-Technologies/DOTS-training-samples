using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TargetingSystem))]
public class ResourceCarrySystem : JobComponentSystem
{
    EntityQuery m_MapQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        return new Job {
            ColonyPosition = map.ColonyPosition,
            ResourcePosition = map.ResourcePosition
        }.Schedule(this, inputDeps);
    }
    
    [BurstCompile]
    struct Job : IJobForEach<PositionComponent, FacingAngleComponent, HasResourcesComponent>
    {
        public float2 ColonyPosition;
        public float2 ResourcePosition;

        public void Execute(
            [ReadOnly] ref PositionComponent position,
            [WriteOnly] ref FacingAngleComponent facingAngle,
            ref HasResourcesComponent hasResources
        )
        {
            float2 target = math.select(ResourcePosition, ColonyPosition, hasResources.Value);
            if (math.lengthsq(position.Value - target) < 4f * 4f) {
                facingAngle.Value += math.PI;
                hasResources.Value = !hasResources.Value;
            }
        }
    }
}
