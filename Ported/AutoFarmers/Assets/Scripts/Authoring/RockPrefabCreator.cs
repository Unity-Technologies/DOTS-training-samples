using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class RockPrefabCreator : MonoBehaviour
{
    public GameObject m_prefab;
    public int NumRocks;
    public int2 RandomSizeMin;
    public int2 RandomSizeMax;
    public float minHeight;
    public float maxHeight;
    public RockState state;
}

public class RockPrefabCreatorBaker : Baker<RockPrefabCreator>
{
    public override void Bake(RockPrefabCreator authoring)
    {
        AddComponent(new rockPrefabCreator
        {
            prefab = GetEntity(authoring.m_prefab),
            NumRocks = authoring.NumRocks,
            RandomSizeMin = authoring.RandomSizeMin,
            RandomSizeMax = authoring.RandomSizeMax,
            minHeight = authoring.minHeight,
            maxHeight = authoring.maxHeight,
            state = authoring.state
        });
    }
}

public struct rockPrefabCreator: IComponentData
{
    public Entity prefab;
    public int NumRocks;
    public int2 RandomSizeMin;
    public int2 RandomSizeMax;
    public float minHeight;
    public float maxHeight;
    public RockState state;
}
