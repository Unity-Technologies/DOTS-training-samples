using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref Translation translation, ref Position position) =>
            {
                translation.Value = new float3(position.Value.x, translation.Value.y, position.Value.y);
            }).ScheduleParallel();
    }
}
