using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct PathMoverComponent : IComponentData
{
    public int2 PathIndices;
    [HideInInspector]
    public int VelocityIdx;
    [HideInInspector]
    public int CurrentPointIndex;
    [HideInInspector]
    public float t;
}
