using Unity.Entities;
using Unity.Mathematics;

struct Point : IComponentData
{
    public float3 pos;
    public float3 old;
    public int neighborCount;

    public Point(in float3 pos)
    {
        old = this.pos = pos;
        neighborCount = 0;
    }
}

struct DynamicPoint : IComponentData {}
struct AnchoredPoint : IComponentData {}

struct Beam : IComponentData
{
    public Entity point1;
    public Entity point2;
    public float3 norm;

    public readonly float length;
    public readonly float thickness;
    public readonly int m1i;
    public readonly int m2i;

    public Beam(in Entity point1, in Entity point2, float thickness, float length, int m1i, int m2i)
    {
        this.point1 = point1;
        this.point2 = point2;
        this.thickness = thickness;
        this.length = length;
        this.m1i = m1i;
        this.m2i = m2i;

        this.norm = float3.zero;
    }
}

