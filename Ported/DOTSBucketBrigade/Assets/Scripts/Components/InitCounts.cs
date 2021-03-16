using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct InitCounts : IComponentData
{
    public int GridSize;
    public int BucketChainCount;
    public int BucketerRadius;
    public int WorkerCountPerChain;
    public int InitialFireInstances;
    public int InitialBucketCount;
    public int WaterSourceCount;
}