using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public enum CurrentDoorState
{
    Close,
    Open
}

[Serializable]
public struct DoorState : IComponentData
{
    public CurrentDoorState value;
}
