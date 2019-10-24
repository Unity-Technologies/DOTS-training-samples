using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct RockTag : IComponentData
{
    public Entity ArmHolding;
}

public struct ForceThrow : IComponentData
{
    public float3 target;
}