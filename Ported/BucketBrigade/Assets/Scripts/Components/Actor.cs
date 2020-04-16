using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[GenerateAuthoringComponent]
public struct Actor : IComponentData
{
    public Entity neighbor;
}

public struct ActorElement : IBufferElementData
{
    public Entity actor;
}