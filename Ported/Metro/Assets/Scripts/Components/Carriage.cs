using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Carriage : IComponentData
{
    public float PositionAlongTrack;
    public int LaneIndex;
    public int NextPlatformIndex;
    
    public Entity NextTrain;
}
