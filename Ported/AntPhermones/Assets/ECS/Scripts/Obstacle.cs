using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

//Obstacle Tag
[GenerateAuthoringComponent]
public struct Obstacle : IComponentData
{
    public float2 position;
    public float radius;
}
