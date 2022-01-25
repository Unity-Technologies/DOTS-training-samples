using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct EntityColor : IComponentData
{
    public Color Value;
}
