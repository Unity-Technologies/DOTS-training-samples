using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using Unity.Rendering;


public class Sys_VelocityJitter : JobComponentSystem
{

[BurstCompile][RequireComponentTag(typeof(Tag_Bee))][ExcludeComponent(typeof(Tag_IsDead), typeof(Tag_IsDying))]
    struct Sys_VelocityJitterJob : IJobForEach<C_Velocity, C_Random>
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public float flightJitter;
        [ReadOnly] public float damping;

        public void Execute(ref C_Velocity velocity, ref C_Random Rand)
        {

            velocity.Value += Rand.Generator.NextFloat3Direction() * (flightJitter * deltaTime);
            velocity.Value *= (1f - damping);
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {

        var job = new Sys_VelocityJitterJob
        {
            deltaTime = UnityEngine.Time.deltaTime,
            flightJitter = BeeManager.S.FlightJitter,
            damping = BeeManager.S.Damping
        };

        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }
}