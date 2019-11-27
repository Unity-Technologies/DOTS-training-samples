using System;
using Unity.Entities;
using UnityEngine;

public class CarSpawnAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Count;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CarSpawnComponent
        {
            Count = Count
        });
    }
}
