using Unity.Entities;
using UnityEngine;

public struct WaterAmount : IComponentData
{
    public byte maxContain;
    public byte currentContain;
}
