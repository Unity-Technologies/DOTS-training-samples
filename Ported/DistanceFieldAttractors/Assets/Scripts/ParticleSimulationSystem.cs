using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
public class ParticleSimulationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var emitterSettings = GetSingleton<ParticleEmitterSettings>();

        Entities
            .WithName("ParticleVariableRateSimulation")
            .ForEach((ref LocalToWorld localToWorld, in Velocity velocity, in Translation translation) =>
        {
            var scale = new float3(.1f, .01f, math.max(.1f, math.length(velocity.Value) * emitterSettings.SpeedStretch));
            var rotation = quaternion.LookRotationSafe(velocity.Value, new float3(0f, 1f, 0f));
            localToWorld.Value = float4x4.TRS(translation.Value, rotation, scale);
        }).ScheduleParallel();
    }
}
