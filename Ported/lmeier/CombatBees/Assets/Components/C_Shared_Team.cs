using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct C_Shared_Team : ISharedComponentData
{
    public Proxy_Bee.Team Team;
}
