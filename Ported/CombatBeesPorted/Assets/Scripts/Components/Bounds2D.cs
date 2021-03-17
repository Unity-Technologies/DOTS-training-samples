using Unity.Entities;
using Unity.Mathematics;

public struct Bounds2D: IComponentData
{
    public float2 Center;
    public float2 Extents;
    
    public bool Contains(float2 position)
    {
        return position.x<Center.x+Extents.x && position.x>Center.x-Extents.x && position.y<Center.y+Extents.y && position.y>Center.y-Extents.y;
    }
}