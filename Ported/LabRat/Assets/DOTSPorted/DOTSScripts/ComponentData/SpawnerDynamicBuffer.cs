using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(1)]
public struct SpawnerDynamicBuffer : IBufferElementData
{
    public int Count;
}
