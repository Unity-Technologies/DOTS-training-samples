using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class RockTagAuthoring : MonoBehaviour
{
}

class RockTagBaker : Baker<RockTagAuthoring>
{
    public override void Bake(RockTagAuthoring authoring)
    {
        AddComponent(new RockTag());
    }
}