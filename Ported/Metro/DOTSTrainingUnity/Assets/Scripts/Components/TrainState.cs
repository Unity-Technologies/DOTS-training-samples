using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public enum CurrTrainState
{
    Waiting,
    Moving,
    Arrived
}

[Serializable]
public struct TrainState : IComponentData
{
    public CurrTrainState value;
}
