using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public struct Random : IComponentData
{
    public Unity.Mathematics.Random random;
}

public class RandomAuthoring : MonoBehaviour
{
    public uint seed;
}

public class RandomBaker : Baker<RandomAuthoring>
{
    public override void Bake(RandomAuthoring authoring)
    {
        AddComponent(new global::Random
        {
            random = Unity.Mathematics.Random.CreateFromIndex(authoring.seed)
        });
        
    }
}
