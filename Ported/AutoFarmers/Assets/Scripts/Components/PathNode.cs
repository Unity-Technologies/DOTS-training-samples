using Unity.Entities;
using Unity.Mathematics;

public struct PathNode : IBufferElementData
{
    PathNode(int x, int y)
    {
        Value = new int2(x, y);
    }
    
    public int2 Value;
}