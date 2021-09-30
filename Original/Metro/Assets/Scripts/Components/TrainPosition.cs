using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TrainPosition : IComponentData
{
    /// <summary>
    /// Position on track is defined in unit points
    /// </summary>
    public float Value;
}
