using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BrigadeRetargetSystem))]
public class BrigadeUpdateSystem : SystemBase
{

    protected override void OnUpdate()
    {
        // move the bots towards their targets
        var deltaTime = Time.DeltaTime;
        Entities
            .ForEach((ref Translation translation, in TargetPosition target) =>
            {
                if (target.Value.Equals(translation.Value))
                {
                    return;
                }
                translation.Value = translation.Value + math.normalize(target.Value - translation.Value) * 1 * deltaTime;
            }).Schedule();

    }
}