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
    [SerializeField] int NbAnts;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(AntPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawner = new AntSpawner
        {
            Origin = transform.position,
            AntPrefab = conversionSystem.GetPrimaryEntity(AntPrefab),
            NbAnts = NbAnts
        };

        dstManager.AddComponentData(entity, spawner);
        dstManager.AddComponent<Direction>(entity);
    }
}
