using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class SizeModifierSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .WithAll<Particle, Grounded>()
            .ForEach((Entity entity, ref Size size, ref NonUniformScale scale, ref Translation translation) =>
            {
                size.Time += deltaTime * size.Speed;
                scale.Value = math.lerp(size.BeginSize, size.EndSize, math.frac(size.Time));
            }).ScheduleParallel();
    }
}