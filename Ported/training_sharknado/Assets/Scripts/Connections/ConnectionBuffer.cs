using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[InternalBufferCapacity(8)]
public struct ConnectionBuffer : IBufferElementData
{
    public Entity entity;
    public int endpoint;
}

