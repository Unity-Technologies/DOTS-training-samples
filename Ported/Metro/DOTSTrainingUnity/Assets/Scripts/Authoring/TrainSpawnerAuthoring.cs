using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TrainSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject TrainEnginePrefab;
    public GameObject TrainCarPrefab;

    public int numberOfTracks;
    public int numberOfCarsPerTrain;
    public int numberOfTrainsPerTrack;

    public UnityEngine.Color[] colors =
        {UnityEngine.Color.red, UnityEngine.Color.green, UnityEngine.Color.yellow, UnityEngine.Color.blue};
    
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
            numberOfTracks = numberOfTracks,
            color0 = new float4(colors[0].r, colors[0].g,colors[0].b, colors[0].a),
            color1 = new float4(colors[1].r, colors[1].g,colors[1].b, colors[1].a),
            color2 = new float4(colors[2].r, colors[2].g,colors[2].b, colors[2].a),
            color3 = new float4(colors[3].r, colors[3].g,colors[3].b, colors[3].a),

        });
    }
}
