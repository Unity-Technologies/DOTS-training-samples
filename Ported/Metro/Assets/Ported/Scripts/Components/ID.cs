using System;
using Unity.Entities;

[Serializable]
public struct ID : ISharedComponentData
{
    public int Value;
}
