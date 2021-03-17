using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Rock a hand is currently trying to grab
/// Grabbing can fail as another hand might have grabbed the hand in the meantime
/// </summary>
[GenerateAuthoringComponent, Serializable]
public struct TargetRock : IComponentData
{
    public Entity RockEntity;
}
