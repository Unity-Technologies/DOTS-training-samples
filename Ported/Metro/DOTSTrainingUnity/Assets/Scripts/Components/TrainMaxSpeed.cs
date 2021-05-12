using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrainMaxSpeed : IComponentData
{
    public float value;
}
