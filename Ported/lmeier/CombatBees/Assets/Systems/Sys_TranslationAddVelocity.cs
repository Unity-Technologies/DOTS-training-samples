using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Transforms;
using static Unity.Mathematics.math;
using Unity.Rendering;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class Sys_TranslationAddVelocity : JobComponentSystem
{

    [BurstCompile][ExcludeComponent(typeof(Tag_IsHeld), typeof(C_Stack))]
    struct Sys_TranslationAddVelocityJob : IJobForEach<Translation,C_Velocity>
    {
        [ReadOnly] public float dt;

        public void Execute(ref Translation trans, [ReadOnly] ref C_Velocity vel)
        {
            trans.Value += vel.Value * dt;
        }
    }
    
    private ProfilerMarker _marker = new ProfilerMarker("Scheduling");
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        _marker.Begin();
        var job = new Sys_TranslationAddVelocityJob
        {
            dt = UnityEngine.Time.deltaTime
        }.Schedule(this, inputDependencies);
        _marker.End();
        
        // Now that the job is set up, schedule it to be run. 
        return job;
    }
}