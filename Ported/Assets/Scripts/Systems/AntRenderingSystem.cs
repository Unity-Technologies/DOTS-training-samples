using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

/**
 * Update the rendering parameters of ants
 */
public partial class AntRenderingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var configuration = GetSingleton<Configuration>();
        Entities.WithAll<AntTag>().ForEach((ref URPMaterialPropertyBaseColor color, in Brightness brightness, in Loadout loadout) =>
        {
            var stateColor = loadout.Value == 0 ? configuration.SearchColor : configuration.CarryColor;
            color.Value += (brightness.Value * stateColor - color.Value) * 0.05f;
        }).ScheduleParallel();
    }
}
