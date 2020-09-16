using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AntTag : IComponentData
{
    public const float Size = 1.5f;
    public bool HasFood;
}
