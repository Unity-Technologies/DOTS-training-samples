using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent, InternalBufferCapacity(8)]
public struct TrainCountBufferElement : IBufferElementData
{
    public int Count;
}