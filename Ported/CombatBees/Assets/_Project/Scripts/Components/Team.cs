using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public enum Team
{
    None = 0,
    TeamA,
    TeamB,
}

[Serializable]
public struct TeamA : IComponentData
{ }

[Serializable]
public struct TeamB : IComponentData
{ }
