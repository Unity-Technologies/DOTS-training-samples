using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TornadoFlagComponent : IComponentData
{
    public Boolean isVortex;   
    
}

[Serializable]
public struct DebrisFlagComponent : IComponentData
{
    public Boolean isDebris;   
    
}
