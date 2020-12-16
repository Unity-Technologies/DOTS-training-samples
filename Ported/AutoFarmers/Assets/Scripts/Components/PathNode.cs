using Unity.Entities;
using Unity.Mathematics;

public struct Path : IComponentData
{
    public int Index;
}

public struct PathNode : IBufferElementData
{
    PathNode(int x, int y)
    {
        Value = new int2(x, y);
    }
    
    public int2 Value;
}