using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class RailSpawnerAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    [SerializeField] private GameObject trackPrefab = null;
    [SerializeField] private GameObject platformPrefab = null;
    [SerializeField] private GameObject trainPrefab = null;
    [SerializeField] private GameObject carriagePrefab = null;
    [SerializeField] private float railSpacing = .2f;
    [SerializeField] private float minAcceleration = .2f;
    [SerializeField] private float maxAcceleration = .2f;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(trackPrefab);
        referencedPrefabs.Add(platformPrefab);
        referencedPrefabs.Add(trainPrefab);
        referencedPrefabs.Add(carriagePrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RailSpawnerComponent
        {
            TrackPrefab =  conversionSystem.GetPrimaryEntity(trackPrefab),
            PlatformPrefab =  conversionSystem.GetPrimaryEntity(platformPrefab),
            TrainPrefab =  conversionSystem.GetPrimaryEntity(trainPrefab),
            CarriagePrefab =  conversionSystem.GetPrimaryEntity(carriagePrefab),
            RailSpacing = railSpacing,
            MinAcceleration = minAcceleration,
            MaxAcceleration = maxAcceleration
        });
    }
}
