using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct C_DeathTimer : IComponentData
{
    public float TimeRemaining;
}
