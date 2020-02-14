using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct PathMoverComponent : IComponentData
{
    public int m_TrackIndex;
    [HideInInspector]
    public int VelocityIdx;
    [HideInInspector]
    public int CurrentPointIndex;
    [HideInInspector]
    public float t;
}
