using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TargetingSystem))]
public class ResourceCarrySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }

    struct BaseJob
    {
        public void Execute([ReadOnly] ref PositionComponent position, [WriteOnly] ref FacingAngleComponent facingAngle)
        {
            
        }
    }
    
    [ExcludeComponent(typeof(HasResourcesTagComponent))]
    struct SearcherJob : IJobForEach<PositionComponent, FacingAngleComponent>
    {
        public BaseJob Data;

        public void Execute([ReadOnly] ref PositionComponent position, [WriteOnly] ref FacingAngleComponent facingAngle)
        {
            Data.Execute(ref position, ref facingAngle);
        }
    }
    
    [RequireComponentTag(typeof(HasResourcesTagComponent))]
    struct CarrierJob : IJobForEach<PositionComponent, FacingAngleComponent>
    {
        public BaseJob Data;
        public void Execute([ReadOnly] ref PositionComponent position, [WriteOnly] ref FacingAngleComponent facingAngle)
        {
            Data.Execute(ref position, ref facingAngle);
        }
    }
}
