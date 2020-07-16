using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public class SeatUpdateSystem : SystemBase
{

    protected override void OnUpdate()
    {
        /*
        var carPositionAccessor = GetComponentDataFromEntity<Translation>(true);
        var carRotationAccessor = GetComponentDataFromEntity<Rotation>(true);
        var seatsPerCar = GetSingleton<SeatsPerCar>();

        //
        //  TODO - Renable schedule and burst
        //
        Entities
            .WithReadOnly(carPositionAccessor)
            .ForEach((ref Translation translation, in Seat seat) =>
        {
            float3 carPosition = carPositionAccessor[seat.car].Value;
            quaternion carRotation = carRotationAccessor[seat.car].Value;
            
            int row = seat.indexInCar / seatsPerCar.cols;
            int col = seat.indexInCar - (row * seatsPerCar.cols);
            
            float3 origin = carPosition;
            float3 rowOffset = seatsPerCar.spacing * new float3(0, 0, -1);
            float3 colOffset = seatsPerCar.spacing * new float3(1, 0, 0);

            float3 position = origin + row * rowOffset + col * colOffset;

            translation.Value = position;
            
            Debug.DrawRay(position, Vector3.up);

        }).WithoutBurst().Run();
        */
    }
}
