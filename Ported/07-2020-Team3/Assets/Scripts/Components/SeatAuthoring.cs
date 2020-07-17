using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SeatAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject[] occupiedBy;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        DynamicBuffer<Seat> seat = dstManager.AddBuffer<Seat>(entity);

        for(int i = 0; i < occupiedBy.Length; i++)
            seat.Add(new Seat { occupiedBy = occupiedBy[i] == null ? Entity.Null : conversionSystem.GetPrimaryEntity(occupiedBy[i])});

    }

    
}
