using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct LocalRotation : IComponentData
{
    public float Value;
}
