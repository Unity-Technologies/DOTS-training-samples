using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnZonesAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public AABB Team1Zone;
    public AABB Team2Zone;
    
    public GameObject BeePrefab;
    public GameObject FoodPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (BeePrefab == null)
        {
            return;
        }
        dstManager.AddComponentData(entity, new SpawnZones()
        {
            Team1Zone = Team1Zone,
            Team2Zone = Team2Zone,
            BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
            FoodPrefab = conversionSystem.GetPrimaryEntity(FoodPrefab),
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BeePrefab);
        referencedPrefabs.Add(FoodPrefab);
    }
}
