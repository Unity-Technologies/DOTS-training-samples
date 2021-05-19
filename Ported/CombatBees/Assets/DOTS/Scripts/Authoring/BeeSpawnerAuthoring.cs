using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class BeeSpawnerAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject BeePrefab;
    [UnityRange(0, 1000)] public int BeeCount;
    
    [UnityRange(0, 100)] public int BeeCountFromResource = 10;

    [UnityRange(0, 100)] public int MaxSpeed = 1;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(BeePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new BeeSpawner
        {
            BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
            BeeCount = BeeCount,
            BeeCountFromResource = BeeCountFromResource,
            MaxSpeed = MaxSpeed
        });
    }
}
