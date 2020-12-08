using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class BeeSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject beePrefab;

    public float3 spawnPosition;
    [Range(0.1f, 10f)]public float spawnRadius;

    [Range( 1, 20)] public int beesToSpawnOnStart;

    public int team;
    public Color teamColour;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(beePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // convert color into float4
        float4 colourNum = new float4(teamColour.r, teamColour.g, teamColour.b, teamColour.a);


        dstManager.AddComponentData(entity, new BeeSpawner
        {
            position = spawnPosition,
            radius = spawnRadius,
            beePrefab = conversionSystem.GetPrimaryEntity(beePrefab),
            teamNumber = team,
            teamColour = colourNum
        });

        dstManager.AddComponentData(entity, new BeeSpawnRequest
        {
            numOfBeesToSpawn = beesToSpawnOnStart
        });
    }
}
