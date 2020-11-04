using System;
using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;

[GenerateAuthoringComponent]
public struct FireSim : IComponentData
{
    public int ChainCount;

    public int BucketCount;
    public int WaterCellCount;

    public int FireGridDimension;
    public int PropagationRadius;
    [Range(0.0f, 1.0f)] public float FlashPoint;
    [Range(0.0f, 1.0f)] public float IgnitionRate;
    [Range(0.0f, 1.0f)] public float HeatTransfer;
}
