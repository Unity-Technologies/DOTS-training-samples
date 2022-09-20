using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

class BeeAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeBaker : Baker<BeeAuthoring>
{
    public override void Bake(BeeAuthoring authoring)
    {
        AddComponent<Velocity>();
    }
}
