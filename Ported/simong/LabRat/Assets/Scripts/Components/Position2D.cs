using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Rotation2D : IComponentData
{
    public float Value;
}