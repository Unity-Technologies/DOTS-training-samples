using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CommuterOnTrainSystem : SystemBase
{

    private EntityQuery _commuterQuery;

    protected override void OnCreate()
    {

        _commuterQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Commuter>()
            }
        });
    }

    protected override void OnUpdate()
    {

        var commuterPositionAccessor = GetComponentDataFromEntity<Translation>(false);
        var commuterRotationAccessor = GetComponentDataFromEntity<Rotation>(false);
        //var seatBufferAccessor = GetBufferFromEntity<Seat>(false);
        var seatsPerCar = GetSingleton<SeatsPerCar>();

        //
        //  TODO - Reenable burst
        //
        Entities
            .WithNativeDisableContainerSafetyRestriction(commuterPositionAccessor)
            .WithNativeDisableContainerSafetyRestriction(commuterRotationAccessor)
            .ForEach((in Entity carEntity, in TrainCar trainCar, in Translation carPosition, in Rotation carRotation, in DynamicBuffer<Seat> seats) =>
        {
            //var seats = seatBufferAccessor[carEntity];

            for (int i = 0; i < seats.Length; i++)
            {
                if (seats[i].occupiedBy != Entity.Null)
                {
                    int col = i / seatsPerCar.cols;
                    int row = i - col * seatsPerCar.cols;

                    float3 offset = (-1 * math.mul(carRotation.Value, new float3(1, 0, 0)) + -2 * math.mul(carRotation.Value, new float3(0, 0, 1)));
                    offset += seatsPerCar.spacing * (col * math.mul(carRotation.Value, new float3(1, 0, 0)) + row * math.mul(carRotation.Value, new float3(0, 0, 1)));

                    var commuterPosition = new Translation
                    {
                        Value = carPosition.Value + seatsPerCar.spacing * offset,
                    };

                    commuterPositionAccessor[seats[i].occupiedBy] = commuterPosition;

                    commuterRotationAccessor[seats[i].occupiedBy] = new Rotation { Value = carRotation.Value };
                }
            }


        }).ScheduleParallel();
        
    }
}
