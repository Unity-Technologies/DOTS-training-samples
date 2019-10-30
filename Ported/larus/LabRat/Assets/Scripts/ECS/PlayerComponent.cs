using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerComponent : IComponentData
{
    [GhostDefaultField]
    public int  PlayerId;
}

public struct LocalPlayerComponent : IComponentData
{
    public int  PlayerId;
}
