using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[GenerateAuthoringComponent]
public struct FireArray : IComponentData
{
    public NativeArray<byte> FireData;
}