using Unity.Entities;
using Unity.Mathematics;

struct Point : IComponentData
{
    public float3 position;
    public float3 old;
    public bool anchor;
    public int neighborCount;

    public Point(float x, float y, float z, bool anchored)
    {
        old = position = new float3(x, y, z);
        anchor = anchored;
        neighborCount = 0;
    }
}

struct Beam : IComponentData
{
    public Entity point1;
    public Entity point2;
    public float3 norm;
    public float length;
    public float thickness;
    public int m1i;
    public int m2i;

    public Beam(in Entity point1, in Entity point2, float thickness, float length)
    {
        this.point1 = point1;
        this.point2 = point2;
        this.thickness = thickness;
        this.length = length;
        m1i = -1;
        m2i = -1;
        norm = float3.zero;
    }
}

