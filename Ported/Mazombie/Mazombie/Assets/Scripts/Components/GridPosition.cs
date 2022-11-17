using Unity.Entities;

public struct GridPositions : IComponentData
{
    public int gridStartX;
    public int gridStartY;
    public int gridEndX;
    public int gridEndY;
}
