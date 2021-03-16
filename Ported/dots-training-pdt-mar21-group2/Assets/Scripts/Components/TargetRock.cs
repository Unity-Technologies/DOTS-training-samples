using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent, Serializable]
public struct TargetRock : IComponentData
{
    public Entity RockEntity;
}
