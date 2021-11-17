using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Bee : IComponentData
{
    public enum ModeCategory
    {
        Searching,
        Hunting, 
        Returning
    };

    public ModeCategory Mode;
    public Entity Carried;
}
