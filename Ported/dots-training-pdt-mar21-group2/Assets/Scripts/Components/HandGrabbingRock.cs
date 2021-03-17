using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// The hand is trying to grab a rock, that is referenced by <seealso cref="TargetRock"/>
/// component. Grabbing can fail as another hand might have grabbed the hand in the meantime
/// </summary>
[Serializable]
public struct HandGrabbingRock : IComponentData
{}

