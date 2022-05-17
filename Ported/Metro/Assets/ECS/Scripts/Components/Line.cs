using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct Line : IComponentData
{
    public FixedString32Bytes Name;
    public Color Colour;
    public int MaxTrains;
    public int CarriagesPerTrain;
    public float MaxTrainSpeed;
}