using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TornadoMovement : SystemBase
{
    protected override void OnUpdate()
    {
        float time = UnityEngine.Time.time;

        Entities.ForEach((ref Translation translation, in Rotation rotation, in TornadoMovementData movementData) => {
            float trackSin = math.sin((time + movementData.loopseed) / movementData.looprate);
            float trackCos = math.cos((time + movementData.loopseed) / movementData.looprate);

            translation.Value.x = trackCos * movementData.loopsize + movementData.loopposition.x;
            translation.Value.z = trackSin * movementData.loopsize + movementData.loopposition.z;
        }).ScheduleParallel();
    }
}
