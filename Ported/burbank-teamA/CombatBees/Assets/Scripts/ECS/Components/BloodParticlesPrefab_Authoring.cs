using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BloodParticlesPrefab_Authoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField] GameObject prefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var prefabEntity = conversionSystem.GetPrimaryEntity(prefab);
        dstManager.AddComponentData(entity, new BloodParticlesPrefab { Value = prefabEntity });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(prefab);
    }
}