using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PassengerSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject[] passengerPrefabs;
    public int numQueues;
    public float passengerSpacing;
    public int passengersPerQueue;
    public float queueSpacing;
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        for(int i=0; i<passengerPrefabs.Length; ++i)
        {
            referencedPrefabs.Add(passengerPrefabs[i]);
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PassengerSpawner()
        { passengerSpacing =passengerSpacing, numQueues = numQueues, passengersPerQueue=passengersPerQueue, queueSpacing = queueSpacing});

        var passengerPrefabsBuffer = dstManager.AddBuffer<PassengerPrefabs>(entity);
        for(int i=0; i<passengerPrefabs.Length; ++i)
        {
            var passengerPrefab = passengerPrefabs[i];
            passengerPrefabsBuffer.Add(conversionSystem.GetPrimaryEntity(passengerPrefab));
        }
    }
}
