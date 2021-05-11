using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct NextTrainEngineRef : IComponentData
{
    public Entity value;
}
