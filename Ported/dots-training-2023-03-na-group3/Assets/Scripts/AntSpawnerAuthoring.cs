using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public struct AntSpawner : IComponentData
{
    public Entity antPrefab;
    public int antAmount;
}

public class AntSpawnerAuthoring : MonoBehaviour
{
    public GameObject antPrefab;
    public int antAmount;
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
    }
}
