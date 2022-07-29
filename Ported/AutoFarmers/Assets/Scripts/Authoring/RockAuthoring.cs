using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class RockAuthoring : MonoBehaviour
{
    public GameObject RockPrefab;
    public int NumRocks;
    public int2 RandomSizeMin;
    public int2 RandomSizeMax;
    public float minHeight;
    public float maxHeight;
   
}

class RockBaker : Baker<RockAuthoring>
{
    public override void Bake(RockAuthoring authoring)
    {
        AddComponent(new RockConfig()
        {
            // By default, each authoring GameObject turns into an Entity.
            // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
            RockPrefab =GetEntity(authoring.RockPrefab),
            NumRocks = authoring.NumRocks,
            RandomSizeMin = authoring.RandomSizeMin,
            RandomSizeMax = authoring.RandomSizeMax,
            minHeight = authoring.minHeight,
            maxHeight = authoring.maxHeight,
            
        });
    }
}