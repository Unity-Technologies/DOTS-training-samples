using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrainTargetDistance : IComponentData
{
    public float value;
}
