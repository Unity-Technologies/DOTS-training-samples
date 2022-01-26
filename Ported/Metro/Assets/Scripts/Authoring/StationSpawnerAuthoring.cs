using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class StationSpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject prefab;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs) {
        referencedPrefabs.Add(prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponentData(entity, new StationSpawner {
            prefab = conversionSystem.GetPrimaryEntity(prefab),
        });
    }
}