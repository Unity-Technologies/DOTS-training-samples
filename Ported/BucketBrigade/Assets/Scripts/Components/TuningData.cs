using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TuningData : IComponentData
{

    public Unity.Mathematics.int2 GridSize;
    public float ValueIncreasePerTick;
    public float MaxValue;
    public float ValuePropagationPerTick;
    public float ValueThreshold;

    public float ActorSpeed;
    public int BucketCapacity;
    public float BucketScale;

}
