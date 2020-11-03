using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveCarOnLaneSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float elapsedTime = Time.fixedDeltaTime;

        Entities
            .ForEach((Entity entity, ref CarPosition carPosition, ref Lane lane, ref CarSpeed carSpeed) =>
            {
                float newPosition = carPosition.Value + carSpeed.Value * elapsedTime;
                while(newPosition > lane.length)
                    newPosition -= lane.length;
                ecb.SetComponent(entity, new CarPosition{Value = newPosition});
            }).Run();

        ecb.Playback(EntityManager);
    }
}