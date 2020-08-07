using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
public class ParticleSimulationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var emitterSettings = GetSingleton<ParticleEmitterSettings>();
        var deltaTime = Time.DeltaTime;

        Entities
            .WithName("ParticleVariableRateSimulation")
            .ForEach((ref LocalToWorld localToWorld, ref Velocity velocity, in Translation translation) =>
        {
            var scale = new float3(.1f, .01f, math.max(.1f, math.length(velocity.Value) * emitterSettings.SpeedStretch));
            var rotation = quaternion.LookRotationSafe(velocity.Value, scale);
            localToWorld.Value = float4x4.TRS(translation.Value, rotation, scale);
        }).ScheduleParallel();
    }
}
