using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public struct TuningData : IComponentData
{

    public Unity.Mathematics.int2 GridSize;
    public int ValueIncreasePerTick;
    public int MaxValue;
    public int ValuePropagationPerTick;
    public int ValueThreshold;

    public float ActorSpeed;
    public int BucketCapacity;
    

}
