using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class Sys_Dying_ResetScale : JobComponentSystem
{

    [BurstCompile][RequireComponentTag(typeof(Tag_IsDying))][ExcludeComponent(typeof(Tag_Particle))]
    struct Sys_Dying_ResetScaleJob : IJobForEach<NonUniformScale, C_Size>
    {
        public void Execute(ref NonUniformScale scale, [ReadOnly] ref C_Size size)
        {
            scale.Value = size.Value;
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new Sys_Dying_ResetScaleJob();
        

        return job.Schedule(this, inputDependencies);
    }
}