using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class PlatformSpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity,IDeclareReferencedPrefabs
{
    
    public UnityGameObject PlatformPrefab;
    
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PlatformPrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlatformSpawner
        {
            PlatformPrefab = conversionSystem.GetPrimaryEntity(PlatformPrefab),
        });
    }
}