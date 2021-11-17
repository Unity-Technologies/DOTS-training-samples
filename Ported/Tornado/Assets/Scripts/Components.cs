using Unity.Entities;
using Unity.Mathematics;

struct Point : IComponentData
{
    public float3 pos;
    public float3 old; // TODO: Split pos, old and start into 3 components
    public float3 start;
    public int neighborCount; // TODO: Put it its own component

    public Point(in float3 pos)
    {
        start = old = this.pos = pos;
        neighborCount = 0;
    }
}

struct DynamicPoint : IComponentData {}
struct AnchoredPoint : IComponentData {}

readonly struct PointDamping : IComponentData 
{
    public readonly float invDamping;
    public readonly float friction;

    public PointDamping(float invDamping, float friction)
    {
        this.invDamping = invDamping;
        this.friction = friction;
    }
}

readonly struct AffectedPoint : IComponentData 
{
    public readonly Entity tornado;
    public readonly float distance;
    public readonly float2 tdir;

    public AffectedPoint(in Entity tornado, in float distance, in float2 dir)
    {
        this.tornado = tornado;
        this.distance = distance;
        this.tdir = dir;
    }
}

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

struct TornadoData : IComponentData
{
    public float force;
    public float maxForceDist;
    public float height;
    public float upForce;
    public float inwardForce;
}

struct TornadoFader : IComponentData
{
    public float fader;
}

readonly struct PointPush : IComponentData
{
    public readonly float3 force;
    public readonly bool breaks;

    public PointPush(in float3 force, in bool breaks)
    {
        this.force = force;
        this.breaks = breaks;
    }
}

readonly struct BeamModif : IComponentData
{
    public readonly int pointIndex;
    public readonly Entity newPointEntity;
    public readonly float3 norm;

    public BeamModif(in int pointIndex, in Entity newPointEntity, in float3 norm)
    {
        this.pointIndex = pointIndex;
        this.newPointEntity = newPointEntity;
        this.norm = norm;
    }
}

struct BeamModif2 : IComponentData
{
    public float3 p1;
    public float3 p2;
    public float3 norm;
    public float3 delta;
    public float dist;
    public bool breaks;
    public int pi;
}

readonly struct SetBeamMatrix : IComponentData
{
    public readonly float3 norm;
    public readonly float3 delta;
    public readonly float dist;

    public SetBeamMatrix(float3 norm, float3 delta, float dist)
    {
        this.norm = norm;
        this.delta = norm;
        this.dist = dist;
    }
}
