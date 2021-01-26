using Unity.Entities;
using UnityEngine;


[GenerateAuthoringComponent]
public struct TrackInfo : IComponentData
{
    public float TrackSize;
    public float SegmentLength;
    public float CornerRadius;

}
