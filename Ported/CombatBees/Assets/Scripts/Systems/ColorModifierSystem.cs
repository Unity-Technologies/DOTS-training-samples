using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

public partial class ColorModifierSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach((Entity entity, ref Color color, ref URPMaterialPropertyBaseColor material) =>
            {
                color.Time += deltaTime * color.Speed;
                material.Value = math.lerp(color.BeginColor, color.EndColor, math.frac(color.Time));
            }).ScheduleParallel();
    }
}
