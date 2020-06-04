using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BaseAnimation : IComponentData
{
    public float Time;
    public float CurrentTime;
}