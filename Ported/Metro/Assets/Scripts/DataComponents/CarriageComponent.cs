using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct CarriageComponent : IComponentData
{
    public int m_CurrentPointIndex;
}
