using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ParticleSimulationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var simulationState = GetSingleton<GenerationSystem.State>();
        var settings = GetSingleton<ParticleSettings>();
        var time = (float)Time.ElapsedTime;
        var deltaTime = Time.DeltaTime;
        
        var job = Entities.ForEach((Entity entity, ref Translation translation, in Particle particle) =>
        {
            var tornadoPos = new float3(simulationState.tornadoX + TornadoJob.TornadoSway(translation.Value.y, time),
                                           translation.Value.y, simulationState.tornadoZ);
            
            var delta = (tornadoPos - translation.Value);
            float dist = math.distance(Vector3.zero, delta);
            delta /= dist;
            
            //TODO: Fix the random here please ->>
            float inForce = dist - math.clamp(tornadoPos.y / 50f, 0f, 1f) * 30f * particle.RandomOffset + 2f;
            translation.Value += new float3(-delta.z * settings.spinRate + delta.x * inForce,
                                            settings.upwardSpeed,
                                            delta.x * settings.spinRate + delta.z * inForce) * deltaTime;
            
            if (translation.Value.y > 50f) {
                translation.Value = new float3(translation.Value.x,0f, translation.Value.z);
            }
            
        }).Schedule(inputDeps);
        
        return job;
    }
}