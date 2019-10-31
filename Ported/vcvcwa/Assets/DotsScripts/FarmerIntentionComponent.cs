using System;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
[GenerateAuthoringComponent]
public struct FarmerIntentionComponent : IComponentData
{
    public FarmerIntention intention;

}