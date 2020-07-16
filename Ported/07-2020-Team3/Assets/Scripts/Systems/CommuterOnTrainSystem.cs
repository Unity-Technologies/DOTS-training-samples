using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
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

        //
        //  TODO - Reenable burst
        //
        Entities
            .ForEach((in Entity carEntity, in TrainCar trainCar, in Translation trainPosition, in DynamicBuffer<Seat> seats) =>
        {
            //var seats = seatBufferAccessor[carEntity];

            for(int i = 0; i < seats.Length; i++)
            {
                if (seats[i].occupiedBy != Entity.Null)
                {
                    var commuterPosition = new Translation
                    {
                        Value = trainPosition.Value
                    };

                    commuterPositionAccessor[seats[i].occupiedBy] = commuterPosition;
                }
            }
            

        }).WithoutBurst().Run();
        
    }
}
