using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrackIndexComponent : ISharedComponentData
{    
    public int m_TrackIndex;
}
