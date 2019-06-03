using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(LateSimulationSystemGroup))][UpdateAfter(typeof(Sys_TranslationAddVelocity))]
public class Sys_LookWhereYouGo : JobComponentSystem
{
    [BurstCompile][RequireComponentTag(typeof(Tag_ILookWhereImGoing))]
    struct Sys_LookWhereYouGoJob : IJobForEach<C_PreviousPos, Translation, Rotation>
    {
        public void Execute(ref C_PreviousPos prevPos, [ReadOnly] ref Translation curPos, ref Rotation rot)
        {
            if(distancesq(curPos.Value,prevPos.Value) > 0.01f)
            {
                rot.Value = Unity.Mathematics.quaternion.LookRotation(curPos.Value - prevPos.Value, math.up());
                prevPos.Value = curPos.Value;
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new Sys_LookWhereYouGoJob();

        return job.Schedule(this, inputDependencies);
    }
}