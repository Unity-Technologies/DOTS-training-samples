using System;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;


[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TornadoSystem : JobComponentSystem
{
    Unity.Mathematics.Random rand;
    EntityQuery generationSettings;
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GenerationSystem.State>();
        rand = new Unity.Mathematics.Random((uint)DateTime.UtcNow.Ticks);
        generationSettings = GetEntityQuery(typeof(GenerationSetting));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps.Complete();
        var settingsEntity = generationSettings.GetSingletonEntity();
        var settings = EntityManager.GetComponentObject<GenerationSetting>(settingsEntity);
        
        var simulationState = GetSingleton<GenerationSystem.State>();
        var time = (float)Time.ElapsedTime;
        var deltaTime = Time.DeltaTime;
        var random = rand;
        
        simulationState.tornadoFader = Mathf.Clamp01(simulationState.tornadoFader + deltaTime / 10f);
        simulationState.tornadoX = Mathf.Cos(time/6f) * 90f;
        simulationState.tornadoZ = Mathf.Sin(time/6f * 1.618f) * 90f;
        
        SetSingleton(simulationState);
        
        Entities.WithoutBurst().ForEach((Entity entity, ref DynamicBuffer<ConstrainedPointEntry> points) =>
        {
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
                
                rand = random,

                points = tempPoints,
            };

            JobHandle jobHandle = job.Schedule(tempPoints.Length, 64);
            jobHandle.Complete();
        }).Run();
        
        return inputDeps;
    }
}
