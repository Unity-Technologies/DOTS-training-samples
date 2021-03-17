using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Idle state : the hand will look for a reachable rock to grab
/// </summary>
[GenerateAuthoringComponent, Serializable]
public struct HandIdle : IComponentData
{}
