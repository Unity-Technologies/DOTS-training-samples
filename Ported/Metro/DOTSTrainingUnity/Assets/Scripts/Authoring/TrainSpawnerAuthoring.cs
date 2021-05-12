using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TrainSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject TrainEnginePrefab;
    public GameObject TrainCarPrefab;

    public int numberOfTracks;
    public int numberOfCarsPerTrain;
    public int numberOfTrainsPerTrack;
    
    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(TrainEnginePrefab);
        referencedPrefabs.Add(TrainCarPrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity trainEnginePrefabEntity = conversionSystem.GetPrimaryEntity(TrainEnginePrefab);
        Entity trainCarPrefabEntity = conversionSystem.GetPrimaryEntity(TrainCarPrefab);

        dstManager.AddComponentData(entity, new TrainSpawnerComponent()
        {
            TrainEnginePrefab = trainEnginePrefabEntity,
            TrainCarPrefab = trainCarPrefabEntity,

            numberOfTrainsPerTrack = numberOfTrainsPerTrack,
            numberOfTrainCarsPerTrain = numberOfCarsPerTrain,
            numberOfTracks = numberOfTracks
        });
    }
}
