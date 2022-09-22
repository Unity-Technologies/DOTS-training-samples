using Unity.Entities;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using Unity.Rendering;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class NestAuthoring : MonoBehaviour
{
    public Factions factions;
}
class NestBaker : Baker<NestAuthoring>
{
    public override void Bake(NestAuthoring authoring)
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        AddSharedComponent<Faction>(new Faction{Value = (int)authoring.factions});
        AddComponent<Area>(new Area{ Value = AABBExtensions.ToAABB(mesh.bounds) });
        AddComponent<URPMaterialPropertyBaseColor>(new URPMaterialPropertyBaseColor{Value = (Vector4)mesh.sharedMaterial.color});
    }
}
