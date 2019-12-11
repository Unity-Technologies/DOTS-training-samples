using Unity.Entities;
using UnityEngine;

public class SimpleMeshRenderer : IComponentData
{
    public Mesh     Mesh;
    public Material Material;
}