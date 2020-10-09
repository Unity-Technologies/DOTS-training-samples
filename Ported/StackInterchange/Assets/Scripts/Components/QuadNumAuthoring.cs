using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

[GenerateAuthoringComponent]
public struct QuadNum : IComponentData
{
    public int QuadID;
}
