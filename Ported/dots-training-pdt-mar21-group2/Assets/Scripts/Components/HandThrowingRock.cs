using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// The rock is grabbed and stored in <seealso cref="TargetRock"/> component.
/// At this point the rock is guaranteed to not be picked by another hand anymore
/// (Exclusive ownership)
/// </summary>
[Serializable]
public struct HandThrowingRock : IComponentData
{}

