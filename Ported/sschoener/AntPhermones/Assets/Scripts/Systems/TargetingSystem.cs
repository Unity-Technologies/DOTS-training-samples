using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TargetingSystem : JobComponentSystem
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
            ResourcePosition = map.ResourcePosition,
        }.Schedule(this, inputDeps);
    }

    struct Job : IJobForEach<PositionComponent, HasResourcesComponent, FacingAngleComponent>
    {
        public float2 ColonyPosition;
        public float2 ResourcePosition;

        public void Execute([ReadOnly] ref PositionComponent position, [ReadOnly] ref HasResourcesComponent hasResources, ref FacingAngleComponent facingAngle)
        {
            throw new System.NotImplementedException();
        }
    }
}
