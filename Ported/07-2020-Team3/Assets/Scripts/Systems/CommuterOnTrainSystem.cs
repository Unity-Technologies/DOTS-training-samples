using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
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
        //var seatBufferAccessor = GetBufferFromEntity<Seat>(false);
        var seatsPerCar = GetSingleton<SeatsPerCar>();

        //
        //  TODO - Reenable burst
        //
        Entities
            .ForEach((in Entity carEntity, in TrainCar trainCar, in Translation carPosition, in Rotation carRotation, in DynamicBuffer<Seat> seats) =>
        {
            //var seats = seatBufferAccessor[carEntity];

            for(int i = 0; i < seats.Length; i++)
            {
                if (seats[i].occupiedBy != Entity.Null)
                {
                    var commuterPosition = new Translation
                    {
                        Value = carPosition.Value + seatsPerCar.spacing * i * math.mul(carRotation.Value, new float3(1, 0, 0)),
                    };

                    commuterPositionAccessor[seats[i].occupiedBy] = commuterPosition;
                }
            }
            

        }).WithoutBurst().Run();
        
    }
}
