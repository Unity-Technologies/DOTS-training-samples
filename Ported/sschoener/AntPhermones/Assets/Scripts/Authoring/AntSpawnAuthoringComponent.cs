using System;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class AntSpawnAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Amount;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AntSpawnComponent
        {
            Amount = Amount
        });
    }
}
