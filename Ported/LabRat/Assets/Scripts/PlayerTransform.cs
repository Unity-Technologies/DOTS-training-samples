using System;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerTransform : IComponentData
{
    public int Index;
}