using System;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
[GenerateAuthoringComponent]
public struct DotsIntentionComponent : IComponentData
{
    public DotsIntention intention;
}