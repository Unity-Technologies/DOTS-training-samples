using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrainTotalDistance : IComponentData
{
    public float value;
}
