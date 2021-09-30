using System.Collections;
using System.Collections.Generic;
using dots_src.Components;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct LineIndex : IComponentData
{
    public Entity entity;
}