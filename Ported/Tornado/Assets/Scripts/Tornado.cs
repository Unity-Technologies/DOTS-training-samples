using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;


[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TornadoSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GenerationSystem.State>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps.Complete();
        
        var settings = GetSingleton<GenerationSetting>();
        var simulationState = GetSingleton<GenerationSystem.State>();
        var time = Time.time;
        var deltaTime = Time.DeltaTime;
        
        
        Entities.ForEach((Entity entity, ref DynamicBuffer<ConstrainedPointEntry> points) =>
        {
            simulationState.tornadoFader = Mathf.Clamp01(simulationState.tornadoFader + deltaTime / 10f);
            simulationState.tornadoX = Mathf.Cos(time/6f) * 30f;
            simulationState.tornadoZ = Mathf.Sin(time/6f * 1.618f) * 30f;

            if (points.Length == 0)
                return;

            // Initialize the job data
            var tempPoints = points.AsNativeArray().Reinterpret<ConstrainedPoint>();
            var job = new TornadoJob
            {
                time = time,
                tornadoForce = settings.tornadoForce,
                tornadoMaxForceDist = settings.tornadoMaxForceDist,
                tornadoHeight = settings.tornadoHeight,
                tornadoUpForce = settings.tornadoUpForce,
                tornadoInwardForce = settings.tornadoInwardForce,
                invDamping = 1f - settings.damping,
                friction = settings.friction,
                
                tornadoX = simulationState.tornadoX,
                tornadoZ = simulationState.tornadoZ,
                tornadoFader = simulationState.tornadoFader,

                points = tempPoints,
            };

            JobHandle jobHandle = job.Schedule(points.Length, 64);
            jobHandle.Complete();
        }).Run();
        
        return inputDeps;
    }
}
