using System;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
public struct ResourcesComponent : IComponentData
{
    public int MoneyForFarmers;
    public int MoneyForDrones;
}