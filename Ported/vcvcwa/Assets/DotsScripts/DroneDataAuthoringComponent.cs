using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class DroneDataAuthoringComponent : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject dronePrefab;

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(dronePrefab);
    }

    // Lets you convert the editor data representation to the entity optimal runtime representation
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new DroneDataComponent
        {
            droneEntity = conversionSystem.GetPrimaryEntity(dronePrefab)
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
}
