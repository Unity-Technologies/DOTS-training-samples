using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TrackProgress : IComponentData
{
    public float Value;
    public int SplineLookupCache;
}
