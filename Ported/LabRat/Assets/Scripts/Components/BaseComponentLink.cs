using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BaseComponentLink : IComponentData
{
    public Entity baseTop;
    public Entity baseBottom;
}
