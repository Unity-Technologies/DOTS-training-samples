using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrainWaiting : IComponentData
{
    public float RemainingTime;

}
