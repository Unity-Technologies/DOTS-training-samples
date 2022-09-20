using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
class BeeAuthoring : MonoBehaviour
{
}

class BeeBaker : Baker<BeeAuthoring>
{
    public override void Bake(BeeAuthoring authoring)
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        
        AddComponent<Bounds>(new Bounds{ Value = AABBExtensions.ToAABB(mesh.bounds) });
    }
}
