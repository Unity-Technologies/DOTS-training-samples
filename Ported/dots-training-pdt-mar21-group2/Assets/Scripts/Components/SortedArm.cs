using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
// Buffer of arms sorted by x position (necessary for spatial queries)
public struct SortedArm : IBufferElementData
{
    public Entity ArmEntity;
}
