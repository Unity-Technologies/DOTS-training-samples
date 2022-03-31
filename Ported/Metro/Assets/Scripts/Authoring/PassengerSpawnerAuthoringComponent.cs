using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PassengerSpawnerAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    [SerializeField] private GameObject passengerPrefab = null;
    [SerializeField] private int passengersPerStation = 50;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(passengerPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PassengerSpawnerComponent
        {
            PassengerPrefab = conversionSystem.GetPrimaryEntity(passengerPrefab),
            PassengersPerStation = passengersPerStation
        });
    }
}