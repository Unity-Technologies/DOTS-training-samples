using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Arc : IComponentData
{
    public const float Size = 1.0f;
    
    public float Radius;
    public float StartAngle;
    public float EndAngle;
}
