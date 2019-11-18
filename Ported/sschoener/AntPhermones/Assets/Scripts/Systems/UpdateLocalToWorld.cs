using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class UpdateLocalToWorld : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }

    struct Job : IJobForEach<FacingAngleComponent, PositionComponent, LocalToWorldComponent>
    {
        public void Execute([ReadOnly] ref FacingAngleComponent angle, [ReadOnly] ref PositionComponent c1, [WriteOnly] ref LocalToWorldComponent c2)
        {
            throw new System.NotImplementedException();
        }
    }
}
