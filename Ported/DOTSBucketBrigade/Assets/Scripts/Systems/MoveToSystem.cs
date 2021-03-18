using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class MoveToSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((Entity entity, ref Translation translation, in TargetPosition target, in Speed speed) =>
            {
                var diff = translation.Value - target.Value;
                if (Mathf.Abs(diff.x) >= 0.01f || Mathf.Abs(diff.z) >= 0.01f)
                {
                    translation.Value.x = Mathf.MoveTowards(translation.Value.x, target.Value.x, speed.Value.x);
                    translation.Value.z = Mathf.MoveTowards(translation.Value.z, target.Value.z, speed.Value.z);
                } 
            }).ScheduleParallel();
    }
}