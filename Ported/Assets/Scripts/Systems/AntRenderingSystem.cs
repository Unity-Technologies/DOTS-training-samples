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
        Entities.WithAll<AntTag>().ForEach((ref URPMaterialPropertyBaseColor color, in Brightness brightness) =>
        {
            color.Value = brightness.Value;
        }).ScheduleParallel();
    }
}
