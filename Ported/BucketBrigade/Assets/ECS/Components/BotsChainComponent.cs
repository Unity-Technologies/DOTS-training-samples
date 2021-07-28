using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BotsChainComponent : IComponentData
{
    public Entity scooper;
    public Entity thrower;
}
