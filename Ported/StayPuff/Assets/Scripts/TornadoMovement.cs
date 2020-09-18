using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[UpdateInGroup(typeof(SimulationSystemGroup),OrderFirst=true)]
[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
public class TornadoMovement : SystemBase
{
    protected override void OnUpdate()
    {
        float time = UnityEngine.Time.time;
        float deltatime = Time.DeltaTime;

        Entities.ForEach((Entity entity,ref Rotation rotation, ref Translation translation, in TornadoMovementData movementData, in TornadoPositionData tornadoPositionData) => {
            float trackSin = math.sin((time + movementData.loopseed) / movementData.looprate);
            float trackCos = math.cos((time + movementData.loopseed) / movementData.looprate);

            translation.Value.x = tornadoPositionData.Position.x+ trackCos * movementData.loopsize + movementData.loopposition.x;
            translation.Value.z = tornadoPositionData.Position.y+ trackSin * movementData.loopsize + movementData.loopposition.z;

        }).ScheduleParallel();
    }
}
