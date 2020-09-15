using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Yaw : IComponentData
{
    public float Value;
}
