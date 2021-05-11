using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrainCurrSpeed : IComponentData
{
    public float value;
}
