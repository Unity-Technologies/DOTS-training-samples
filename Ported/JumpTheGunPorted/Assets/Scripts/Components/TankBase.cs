using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TankBase : IComponentData
{
    public const float Y_OFFSET = 0.0f;
    public const float TURRET__Y_OFFSET = 0.7f; // compensate for turret being up higher
}
