using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class AnimateEntitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime; 

        Entities
            .WithAll<AnimationProperties>()
            .ForEach((Entity entity, ref Scale scale, in AnimationProperties animationProperties) =>
            {
                var scaleFactor = animationProperties.ObjScaleFactor;
                var animTime = animationProperties.ObjectScaleTime;

                scale.Value = Unity.Mathematics.math.lerp(scale.Value, scale.Value * scaleFactor, animTime * dt);

            }).Run();
    }
}
