using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;


public struct AntSpawner : IComponentData
{
    public Entity antPrefab;
    public int antAmount;
}
public struct Random : IComponentData
{
    public Unity.Mathematics.Random randomSeed;
}

public class AntSpawnerAuthoring : MonoBehaviour
{
    public GameObject antPrefab;
    public int antAmount;
    public uint randomSeed;
}

public class AntSpawnerBaker : Baker<AntSpawnerAuthoring>
{
    public override void Bake(AntSpawnerAuthoring authoring)
    {
        AddComponent(new AntSpawner
        {
            antPrefab = GetEntity(authoring.antPrefab),
            antAmount = authoring.antAmount
        });
        AddComponent(new Random
        {
            randomSeed = Unity.Mathematics.Random.CreateFromIndex(authoring.randomSeed)
        });
    }
}
