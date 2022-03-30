using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PlatformSpawnerAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    [SerializeField] private GameObject platformPrefab = null;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(platformPrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlatformSpawnerComponent
        {
            PlatformPrefab = conversionSystem.GetPrimaryEntity(platformPrefab)
        });
    }
}
