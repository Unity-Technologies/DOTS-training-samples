using Unity.Entities;
using Unity.Mathematics;

public struct TempCarriageDestination : IComponentData, IEnableableComponent
{
    public float3 TempDestination;
}