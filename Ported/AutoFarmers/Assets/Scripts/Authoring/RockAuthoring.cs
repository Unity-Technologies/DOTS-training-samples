using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class RockAuthoring : MonoBehaviour
{
    public int NumRocks;
    public int2 RandomSizeMin;
    public int2 RandomSizeMax;
    public float minHeight;
    public float maxHeight;
    public RockState state;
}

class RockBaker : Baker<RockAuthoring>
{
    public override void Bake(RockAuthoring authoring)
    {
        AddComponent(new RockTag());

        AddComponent(new RockConfig()
        {
            // By default, each authoring GameObject turns into an Entity.
            // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
            NumRocks = authoring.NumRocks,
            RandomSizeMin = authoring.RandomSizeMin,
            RandomSizeMax = authoring.RandomSizeMax,
            minHeight = authoring.minHeight,
            maxHeight = authoring.maxHeight,
            state = authoring.state,
        });
    }
}