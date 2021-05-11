using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrainEngineRef : IComponentData
{
    public Entity value;
}
