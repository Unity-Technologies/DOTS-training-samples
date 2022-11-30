using System.Collections;
using System.Collections.Generic;
using Systems;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

// [BurstCompile]
// [WithAll(typeof(Train))]
// partial struct PassengerMovementJob : IJobEntity
// {
//     // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
//     public float DeltaTime;
//
//     void Execute(ref TrainAspect train)
//     {
//         var direction = train.TrainDestination - train.Position;
//         //train.Train.ValueRW.DestinationDirection = direction;
//         var trainDirection = train.Forward;
//         //train.Train.ValueRW.Forward = trainDirection;
//         var angle = Utility.Angle(trainDirection, direction);
//         //train.Train.ValueRW.Angle = angle;
//         if(angle > 0.01f)
//             train.Rotation = quaternion.RotateY(angle);
//             
//             
//         var distanceToThePoint = math.lengthsq(direction);
//         if (distanceToThePoint > 0.001f)
//         {
//             train.Position += math.normalize(direction) * (DeltaTime * train.CurrentSpeed);
//         }
//     }
// }

[BurstCompile]
[RequireMatchingQueriesForUpdate]
partial struct PassengerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Random random = new Random(1234);
        foreach ((var passenger, var entity) in SystemAPI.Query<Passenger>().WithEntityAccess())
        {
            var transform = SystemAPI.GetComponent<LocalTransform>(entity);
            if (passenger.State == PassengerState.Waiting)
            {
                SystemAPI.SetComponent<Passenger>(entity, new Passenger()
                {
                    State = PassengerState.Walking,
                    Destination = new float3(transform.Position.x + random.NextFloat(-5,5),0,transform.Position.z + random.NextFloat(-5,5))
                });
            }
            else if (passenger.State == PassengerState.Walking)
            {
                float3 dir = passenger.Destination - transform.Position;
                var distanceToThePoint = math.lengthsq(dir);
                if (distanceToThePoint > 0.001f) 
                {
                    var pos = transform.Position + math.normalize(dir) * 3 * SystemAPI.Time.DeltaTime;
                    SystemAPI.SetComponent<LocalTransform>(entity, LocalTransform.FromPosition(pos));
                }
                else
                {
                    SystemAPI.SetComponent<Passenger>(entity, new Passenger()
                    {
                        State = PassengerState.Waiting
                    });
                }
            }
        }
    }
}
