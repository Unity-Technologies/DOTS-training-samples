using Unity.Entities;

[GenerateAuthoringComponent]
public struct FireSim : IComponentData
{
    public int BucketCount;
    public int WaterCellCount;

    public int FireGridDimension;
    public int PropagationRadius;
    public byte FlashPoint;
    public float IgnitionRate;
    public float HeatTransfer;
}
