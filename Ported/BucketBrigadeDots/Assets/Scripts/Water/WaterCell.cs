using Unity.Entities;

public struct WaterCell : IComponentData
{
    // TODO: resize water cells based on their current value.
    public float WaterValue;
    public float MaxWaterValue;
}