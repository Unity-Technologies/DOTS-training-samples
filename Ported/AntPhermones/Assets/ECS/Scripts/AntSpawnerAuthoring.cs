using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AntSpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [SerializeField] GameObject AntPrefab;
    [SerializeField] GameObject ColonyPrefab;
    [SerializeField] GameObject FoodPrefab;
    [SerializeField] Transform ColonyTransform;
    [SerializeField] Transform FoodTransform;
    [SerializeField] int NbAnts;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(AntPrefab);
        referencedPrefabs.Add(ColonyPrefab);
        referencedPrefabs.Add(FoodPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawner = new AntSpawner
        {
            Origin = transform.position,
            ColonyPosition = ColonyTransform.position,
            FoodPosition = FoodTransform.position,
            AntPrefab = conversionSystem.GetPrimaryEntity(AntPrefab),
            ColonyPrefab = conversionSystem.GetPrimaryEntity(ColonyPrefab),
            FoodPrefab = conversionSystem.GetPrimaryEntity(FoodPrefab),
            NbAnts = NbAnts
        };

        dstManager.AddComponentData(entity, spawner);
        dstManager.AddComponent<Direction>(entity);
    }
}
