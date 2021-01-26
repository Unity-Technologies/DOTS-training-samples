using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InitializationSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject BeePrefab;
    [Range(0, 10000)] public int NumberOfBees;
    
    public GameObject FoodPrefab;
    [Range(0, 10000)] public int NumberOfFood;
    public AABB FoodSpawnBox;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (BeePrefab == null)
        {
            return;
        }
        dstManager.AddComponentData(entity, new InitializationSpawner()
        {
            BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
            NumberOfBees = NumberOfBees,
            FoodPrefab = conversionSystem.GetPrimaryEntity(FoodPrefab),
            NumberOfFood = NumberOfFood,
            FoodSpawnBox = FoodSpawnBox,
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BeePrefab);
        referencedPrefabs.Add(FoodPrefab);
    }
}
