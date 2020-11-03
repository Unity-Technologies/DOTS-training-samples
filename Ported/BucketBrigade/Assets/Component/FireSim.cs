using Unity.Entities;

[GenerateAuthoringComponent]
public struct FireSim : IComponentData
{
    public int BucketCount;
    public int FireGridDimension;
    public int WaterCellCount;
    public byte FlashPoint; 
}
