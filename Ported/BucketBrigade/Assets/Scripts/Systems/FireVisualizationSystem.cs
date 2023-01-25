using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Random = UnityEngine.Random;

partial class FireVisualizationSystem : SystemBase
{
    
    protected override void OnUpdate()
    {
        var speed = 1;
        var offsetRange = 0.2f;
        var runningTime = ((float)SystemAPI.Time.ElapsedTime * speed) * offsetRange;
        
        Entities
            .WithAll<OnFireTag>()
            .ForEach((ref TransformAspect transform) =>
            {
                var pos = transform.LocalPosition;
                pos.y = Mathf.Sin(runningTime);
                transform.LocalPosition = pos;

            }).Run();

        // Entities
        //     .WithAll<DisplayHeight>()
        //     .ForEach((URPMaterialPropertyBaseColor color) =>
        //     {
        //         color = new URPMaterialPropertyBaseColor();
        //     })
        //     .ScheduleParallel();
    }
}




