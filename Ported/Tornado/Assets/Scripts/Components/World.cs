using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct World : IComponentData
{ }


struct BeamBatch : ISharedComponentData
{
    public int Value;
    public int pointStartIndex;
    public int pointEndIndex;
}