using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class SpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BeePrefab;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BeePrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Spawner
        {
            BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab)
        });
    }
}