using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AntMovimentSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = Time.DeltaTime;
        var random = new Random(244);
        
        Entities.WithAll<Ant>().ForEach((ref Translation translation) =>
        {
            translation.Value.x += random.NextFloat(-10, 10) * time;
            translation.Value.y += random.NextFloat(-10, 10) * time;
        }).ScheduleParallel();
    }
}
