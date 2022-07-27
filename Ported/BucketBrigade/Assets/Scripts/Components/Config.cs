using Unity.Entities;

// An empty component is called a "tag component".
struct Config : IComponentData
{
    public int GridSize;
    public int InitialFireCount;
    public float FireThreshold;
    public int FireFighterLinesCount;
    public int FireFighterPerLineCount;
}