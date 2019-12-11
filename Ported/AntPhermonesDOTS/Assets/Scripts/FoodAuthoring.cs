using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FoodAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject prefab;
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(prefab);
    }
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Food
        {
            Prefab = conversionSystem.GetPrimaryEntity(prefab)
        });
    }
}