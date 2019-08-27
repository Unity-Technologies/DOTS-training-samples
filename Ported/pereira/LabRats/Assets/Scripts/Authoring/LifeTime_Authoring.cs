using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class LifeTime_Authoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float LifeTime = 3;
    
    // The MonoBehaviour data is converted to ComponentData on the entity.
    // We are specifically transforming from a good editor representation of the data (Represented in degrees)
    // To a good runtime representation (Represented in radians)
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LbLifetime { Value = LifeTime });
        dstManager.AddComponentData(entity, new LbMovementSpeed { Value = 0 });
        dstManager.AddComponentData(entity, new LbFall());
    }
}
