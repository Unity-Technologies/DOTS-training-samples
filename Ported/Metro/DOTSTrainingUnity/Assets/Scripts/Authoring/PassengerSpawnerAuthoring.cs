using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PassengerSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject passengerPrefab;
    public int numQueues;
    public float passengerSpacing;
    public int passengersPerQueue;
    public float queueSpacing;
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(passengerPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PassengerSpawner()
        { passengerSpacing=passengerSpacing, numQueues = numQueues, passengersPerQueue=passengersPerQueue, queueSpacing = queueSpacing, passengerPrefab = conversionSystem.GetPrimaryEntity(passengerPrefab)});
    }
}
