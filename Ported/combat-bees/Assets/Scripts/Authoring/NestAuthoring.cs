using Unity.Entities;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class NestAuthoring : MonoBehaviour
{
}
class NestBaker : Baker<NestAuthoring>
{
    public override void Bake(NestAuthoring authoring)
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        
        AddComponent<Bounds>(new Bounds{ Value = AABBExtensions.ToAABB(mesh.bounds) });
    }
}
