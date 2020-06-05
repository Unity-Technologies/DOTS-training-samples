using Unity.Entities;

[GenerateAuthoringComponent]
public struct WaterLevel : IComponentData
{
    public float Capacity;
    public float Level;

    public bool Full => Level >= Capacity;
}
