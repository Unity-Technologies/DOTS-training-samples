using System;
using Unity.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct MetroData : IComponentData
{
    public Entity PlatformPrefab;
    public Entity RailMarkerPrefab;
    public Entity CommuterPrefab;
}
