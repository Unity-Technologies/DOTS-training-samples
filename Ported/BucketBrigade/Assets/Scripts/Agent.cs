using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Agent : IComponentData
{
    public int TeamID;
    public Entity CarriedEntity;
    public float MaxVelocity;
}

public struct AgentTags // for organization, mainly.
{
    public struct ScooperTag : IComponentData
    {

    }

    public struct ThrowerTag : IComponentData
    {
    
    }

    public struct FullBucketPasserTag : IComponentData
    {
    
    }

    public struct EmptyBucketPasserTag : IComponentData
    {
    
    }

    public struct OmniBotTag : IComponentData
    {
    
    }
}