using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrainCurrDistance : IComponentData
{
    public float value;
}
