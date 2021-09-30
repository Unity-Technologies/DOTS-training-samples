using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class SizeModifierSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<Particle, Grounded>()
            .ForEach((Entity entity, ref Size size, ref NonUniformScale scale, ref Translation translation, in LifeTime lifetime) =>
            {
                float t = lifetime.TimeRemaining / lifetime.TotalTime;
                scale.Value = math.lerp(size.BeginSize, size.EndSize, 1.0f - t);
            }).ScheduleParallel();
    }
}