using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.Serialization;

[RequiresEntityConversion]
public class Spawner_Authoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject MousePrefab;
    public float MouseFrequency;
    
    public GameObject CatPrefab;
    public float CatFrequency;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(MousePrefab);
        referencedPrefabs.Add(CatPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new LbSpawner
        {
            MousePrefab = conversionSystem.GetPrimaryEntity(MousePrefab),
            MouseFrequency = MouseFrequency,
            
            CatPrefab = conversionSystem.GetPrimaryEntity(CatPrefab),
            CatFrequency = CatFrequency
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
}
