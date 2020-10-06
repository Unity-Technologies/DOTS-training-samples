using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public struct CarSize : IComponentData
{
    public float length;
    public float width;
    public float height;
}