using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Line : IComponentData
{
    public Translation fillTranslation;
    public Translation tossTranslation;
}
