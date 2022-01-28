using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(CarMovementGroup))]
public partial class CarMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Persistent);
        
        var singleton = GetSingletonEntity<SplineDefArrayElement>();
        var splineIdToRoadArray = GetBuffer<SplineIdToRoad>(singleton);

        
        Entities
            .WithAll<SplinePosition, SplineDef, Speed>()
            .WithNone<RoadCompleted>()
            .ForEach((Entity entity, ref SplinePosition splinePosition, in SplineDef splineDef, in Speed speed, in Translation position) =>
            {
                var roadQueue = GetBuffer<CarQueue>(splineIdToRoadArray[splineDef.splineId].Value);
                int ownIndex = 0;
                for (var i = 0; i < roadQueue.Length; i++)
                {
                    if (roadQueue[i] == entity)
                    {
                        ownIndex = i;
                        break;
                    }
                }
                
                if (ownIndex > 0)
                {
                    var frontCarPosition = GetComponent<Translation>(roadQueue[ownIndex-1].Value);
                    if (SqrMagnitude(frontCarPosition.Value, position.Value) < 0.15f) //TODO add car length to const comp
                        return;
                }
                
                var splineVector = (splineDef.endPoint - splineDef.startPoint);
                splinePosition.position += speed.speed * time * 1 / math.length(splineVector);
                if (splinePosition.position > 1.0f)
                {
                    splinePosition.position = 1.0f;
                    ecb.AddComponent<RoadCompleted>(entity);
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    private static float SqrMagnitude(float3 start, float3 end)
    {
        var diff = end - start;
        return math.abs(diff.x * diff.x + diff.y * diff.y + diff.z * diff.z);
    }
}
