using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Animation timer for each phase of the arm (grab, wind up, throw)
/// It decreases, meaning the phase is finished when timer reach 0
/// </summary>
[GenerateAuthoringComponent, Serializable]
public struct Timer : IComponentData
{
    public float Value;
}
