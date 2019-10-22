using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct CarSharedData : ISharedComponentData
{
    public float distanceToFront;
    public float distanceToBack;
    public Color defaultColor;
    public Color maxSpeedColor;
    public Color minSpeedColor;
}
