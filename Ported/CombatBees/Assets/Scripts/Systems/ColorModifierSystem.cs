using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

public partial class ColorModifierSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((Entity entity, ref Color color, ref URPMaterialPropertyBaseColor material, in LifeTime lifetime) =>
            {
                float t = lifetime.TimeRemaining / lifetime.TotalTime;
                material.Value = math.lerp(color.BeginColor, color.EndColor, 1.0f - t);
            }).ScheduleParallel();
    }
}