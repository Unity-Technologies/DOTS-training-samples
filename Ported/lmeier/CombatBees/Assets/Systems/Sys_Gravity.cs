using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
[UpdateInGroup(typeof(LateSimulationSystemGroup))][UpdateBefore(typeof(Sys_TranslationAddVelocity))]
public class Sys_Gravity : JobComponentSystem
{

    [BurstCompile][ExcludeComponent(typeof(Tag_Bee),typeof(Tag_Sticky), typeof(C_Stack))]
    struct Sys_GravityJob : IJobForEach<C_Velocity>
    {
        [ReadOnly] public float dt;
        [ReadOnly] public float gravity;

        public void Execute(ref C_Velocity velocity)
        {
            velocity.Value.y += gravity * dt;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new Sys_GravityJob()
        {
            dt = UnityEngine.Time.deltaTime,
            gravity = GameConstants.S.Gravity
        };

        return job.Schedule(this, inputDependencies);
    }
}