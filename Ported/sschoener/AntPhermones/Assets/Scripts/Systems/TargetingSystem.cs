using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TargetingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }

    struct BaseJob
    {
        public void Execute([ReadOnly] ref PositionComponent c0, ref FacingAngleComponent c1)
        {
            throw new System.NotImplementedException();
        }
    }
    
    [ExcludeComponent(typeof(HasResourcesTagComponent))]
    struct SearcherJob : IJobForEach<PositionComponent, FacingAngleComponent>
    {
        public BaseJob Data;

        public void Execute([ReadOnly] ref PositionComponent c0, ref FacingAngleComponent c1)
        {
            throw new System.NotImplementedException();
        }
    }
    
    [RequireComponentTag(typeof(HasResourcesTagComponent))]
    struct CarrierJob : IJobForEach<PositionComponent, FacingAngleComponent>
    {
        public BaseJob Data;
        public void Execute([ReadOnly] ref PositionComponent c0, ref FacingAngleComponent c1)
        {
            throw new System.NotImplementedException();
        }
    }
}
