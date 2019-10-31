using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class ResourcesAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public int moneyForFarmers;
    public int moneyForDrones;

    // Lets you convert the editor data representation to the entity optimal runtime representation
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new ResourcesComponent(){MoneyForDrones = moneyForDrones, MoneyForFarmers = moneyForFarmers};
        dstManager.AddComponentData(entity, data);
    }
}
