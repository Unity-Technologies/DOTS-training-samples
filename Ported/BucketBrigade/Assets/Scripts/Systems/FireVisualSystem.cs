using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FireVisualSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!HasSingleton<TuningData>())
        {
            return;
        }

        TuningData tuningData = GetSingleton<TuningData>();
        var maxValue = tuningData.MaxValue;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithAll<Fire>().ForEach(
            (ref FireMaterialComponent mat, ref Translation translation, in ValueComponent val) =>
            {
                //map the overall value from [0-1]
                float intensity = val.Value / maxValue;
                translation.Value.y = intensity - 0.5f;
                mat.Amount = 255f * intensity;
                
            }).Run();

        ecb.Playback(EntityManager);
    }
}