using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class Sys_Bee_ScaleByVel : JobComponentSystem
{
    [BurstCompile][RequireComponentTag(typeof(Tag_StretchByVelocity))][ExcludeComponent(typeof(Tag_IsDying))]
    struct Sys_Bee_ScaleByVelJob : IJobForEach<NonUniformScale, C_Size, C_Velocity>
    {
        [ReadOnly] public float SpeedStretch;
       
        public void Execute(ref NonUniformScale Scale, ref C_Size Size, [ReadOnly] ref C_Velocity Velocity)
        {
            float3 scale = float3(Size.Value);

            float stretch = clamp(length(Velocity.Value) * SpeedStretch, 1.0f, 10.0f);
            scale.z *= stretch;
            scale.x /= (stretch - 1f) / 5f + 1f;
            scale.y /= (stretch - 1f) / 5f + 1f;

            Scale.Value = scale;
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new Sys_Bee_ScaleByVelJob()
        {
            SpeedStretch = BeeManager.S.SpeedStretch
        };
 
        return job.Schedule(this, inputDependencies);
    }
}