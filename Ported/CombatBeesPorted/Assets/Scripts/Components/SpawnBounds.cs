using Unity.Entities;
using Unity.Mathematics;

public struct SpawnBounds: IComponentData
{
    public float3 Center;
    public float3 Extents;
    
    public bool Contains(float3 position)
    {
        return position.x<Center.x+Extents.x && position.x>Center.x-Extents.x && position.y<Center.y+Extents.y && position.y>Center.y-Extents.y && position.z<Center.z+Extents.z && position.z>Center.z-Extents.z;
    }
}