using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SeatAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        DynamicBuffer<Seat> seat = dstManager.AddBuffer<Seat>(entity);

        //
        //  TODO - Don't hardcode this
        //
        for(int i = 0; i < 15; i++)
            seat.Add(new Seat { occupiedBy = Entity.Null });

    }
}
