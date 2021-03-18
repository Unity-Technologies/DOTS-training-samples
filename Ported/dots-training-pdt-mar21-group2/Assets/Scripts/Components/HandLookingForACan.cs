using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Stay in that state until we find a can to throw a rock at
/// </summary>
[Serializable]
public struct HandLookingForACan : IComponentData
{}

