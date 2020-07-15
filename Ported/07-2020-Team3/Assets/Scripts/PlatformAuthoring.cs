using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Platform : IComponentData
{
    public float3 HeadStairsBottom;
    public float3 HeadStairsTop;
    public float3 FootStairsBottom;
    public float3 FootStairsTop;
    public Entity NextPlatform;
}
