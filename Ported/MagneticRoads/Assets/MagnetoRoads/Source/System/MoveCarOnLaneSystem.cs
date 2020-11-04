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

        ComponentDataFromEntity<CarPosition> carPositionGetter = GetComponentDataFromEntity<CarPosition>(false);
        ComponentDataFromEntity<CarSpeed> carSpeedGetter = GetComponentDataFromEntity<CarSpeed>(false);
        
        Entities
            .ForEach((Entity entity, ref Lane lane, ref DynamicBuffer<MyBufferElement> buffer) =>
            {
                foreach (Entity car in buffer.Reinterpret<Entity>())
                {
                    CarPosition carPosition = carPositionGetter[car];
                    CarSpeed carSpeed = carSpeedGetter[car];
                    float newPosition = carPosition.Value + carSpeed.Value * elapsedTime;
                    if(newPosition > lane.Length)
                        newPosition = lane.Length;
                    ecb.SetComponent(car, new CarPosition{Value = newPosition});
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}