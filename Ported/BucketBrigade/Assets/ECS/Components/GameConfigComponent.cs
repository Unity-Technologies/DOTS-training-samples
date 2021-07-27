using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct GameConfigComponent : IComponentData
{
    public float WaterRefillRate;
    public float WaterMaxScale;
}
