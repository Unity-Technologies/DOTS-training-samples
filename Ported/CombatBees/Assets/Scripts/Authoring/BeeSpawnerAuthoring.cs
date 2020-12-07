using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class BeeSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject beePrefab;
    public int initialBeeCount;
    public float maxSpawnSpeed;
    public BeeTeam.TeamColor team;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawner = new BeeSpawner
        {
            beePrefab = conversionSystem.GetPrimaryEntity(this.beePrefab),
            count = this.initialBeeCount,
            maxSpawnSpeed = this.maxSpawnSpeed,
            team = this.team
        };

        dstManager.AddComponentData<BeeSpawner>(entity, spawner);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(this.beePrefab);
    }
}

public struct BeeSpawner : IComponentData
{
    public Entity beePrefab;
    public int count;
    public float maxSpawnSpeed;
    public BeeTeam.TeamColor team;
}