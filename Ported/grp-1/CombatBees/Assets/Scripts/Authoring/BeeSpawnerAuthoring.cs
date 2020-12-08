using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class BeeSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject beePrefab;

    [Range( 1, 20)] public int numBeesToSpawn;

    public int team;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(beePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeSpawner
        {
            beePrefab = conversionSystem.GetPrimaryEntity(beePrefab),
            numBeesToSpawn = numBeesToSpawn,
            teamNumber = team
        });
    }
}
