using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Carriage : IComponentData
{
    public int num;
    public Entity NextTrain;
    public float PositionAlongTrack;
    public int LaneIndex;
    public int NextPlatformIndex;
}
