using System.Collections.Generic;
using System.Collections;
using Unity.Entities;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class MouseAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Directions Direction;
    public float Speed = 2;

    // The MonoBehaviour data is converted to ComponentData on the entity.
    // We are specifically transforming from a good editor representation of the data (Represented in degrees)
    // To a good runtime representation (Represented in radians)
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LbRat());
        dstManager.AddComponentData(entity, new LbMovementSpeed() { Value = Speed });
        dstManager.AddComponentData(entity, new LbDistanceToTarget() { Value = 1.0f });
        dstManager.AddComponentData(entity, new LbDirection() { Value = (byte)Direction });
        dstManager.AddComponentData(entity, new LbMovementTarget());
        
    }
}
