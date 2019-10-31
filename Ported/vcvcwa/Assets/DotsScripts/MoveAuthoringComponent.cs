using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class MoveAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool fly;

    // Lets you convert the editor data representation to the entity optimal runtime representation
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new MoveComponent{fly = fly};
        dstManager.AddComponentData(entity, data);
    }
}
