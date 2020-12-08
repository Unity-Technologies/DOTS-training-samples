using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class BeeSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject beePrefab;

    [Range( 1, 20)] public int numBeesToSpawn;

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
            beePrefab = conversionSystem.GetPrimaryEntity(beePrefab),
            numBeesToSpawn = numBeesToSpawn,
            teamNumber = team,
            teamColour = colourNum
        });
    }
}
