using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct FireSpreadSettings : IComponentData
{
    public float fireSimUpdateRate;
    public float heatTransferRate;
    public float flashpoint;
    public int heatRadius;
    public int splashRadius;
    public float coolStrength;
    public int coolRadious;
}
