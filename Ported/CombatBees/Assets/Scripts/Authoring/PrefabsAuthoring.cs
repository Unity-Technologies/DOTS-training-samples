using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class PrefabsAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BloodPrefab;
    public UnityGameObject SpawnCloudPrefab;
    public UnityGameObject BlueBeePrefab;
    public UnityGameObject RedBeePrefab;
    public UnityGameObject FoodPrefab;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(BloodPrefab);
        referencedPrefabs.Add(SpawnCloudPrefab);
        referencedPrefabs.Add(FoodPrefab);
        referencedPrefabs.Add(BlueBeePrefab);
        referencedPrefabs.Add(RedBeePrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new Prefabs
        {
            BloodPrefab = conversionSystem.GetPrimaryEntity(BloodPrefab),
            SpawnCloudPrefab = conversionSystem.GetPrimaryEntity(SpawnCloudPrefab),
            BlueBeePrefab = conversionSystem.GetPrimaryEntity(BlueBeePrefab),
            RedBeePrefab = conversionSystem.GetPrimaryEntity(RedBeePrefab),
            FoodPrefab = conversionSystem.GetPrimaryEntity(FoodPrefab),

        });
    }
}