using System;
using Unity.Entities;

[Serializable]
public struct RailMarker : IComponentData
{
    public int Index;
    public int MarkerType;
}