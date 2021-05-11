using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrainWaitTimer : IComponentData
{
    public int value;
}
