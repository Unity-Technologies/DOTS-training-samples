using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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

struct AffectedPoint : IComponentData 
{
    public bool enabled;
    public readonly float distance;
    public readonly float2 tdir;
    public readonly float fader;
    public readonly float force;
    public readonly float height;
    public readonly float inwardForce;
    public readonly float upForce;
    public readonly float maxForceDist;

    public AffectedPoint(in TornadoState ts, in float distance, in float2 dir)
    {
        enabled = true;
        this.distance = distance;
        this.tdir = dir;
        this.fader = ts.fader;
        this.force = ts.data.force;
        this.height = ts.data.height;
        this.inwardForce = ts.data.inwardForce;
        this.upForce = ts.data.upForce;
        this.maxForceDist = ts.data.maxForceDist;
    }
}

struct Beam : IComponentData
{
    public Entity point1;
    public Entity point2;
    public float3 norm;
    public unsafe Matrix4x4* matrix; // TODO: Move to its own component

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

        unsafe
        {
            matrix = null;
        }
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

readonly struct TornadoFader : IComponentData
{
    public readonly float value;

    public TornadoFader(in float value)
    {
        this.value = value;
    }
}

struct BeamModif : IComponentData
{
    public bool enabled;
    public float3 p1;
    public float3 p2;
    public float3 norm;
    public float3 delta;
    public float dist;
    public bool breaks;
    public int pi;
}

static class ArcheTypes
{
    public static readonly ComponentType[] FixedPointTypes = new ComponentType[] { typeof(Point), typeof(PointDamping), typeof(AnchoredPoint) };
    public static readonly ComponentType[] DynamicPointTypes = new ComponentType[] { typeof(Point), typeof(PointDamping), typeof(DynamicPoint), typeof(AffectedPoint) };
    public static readonly ComponentType[] BeamTypes = new ComponentType[] { typeof(Beam), typeof(BeamModif) };

    public static readonly EntityArchetype Beam;
    public static readonly EntityArchetype FixedPoint;
    public static readonly EntityArchetype DynamicPoint;

    static ArcheTypes()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        Beam = em.CreateArchetype(BeamTypes);
        FixedPoint = em.CreateArchetype(FixedPointTypes);
        DynamicPoint = em.CreateArchetype(DynamicPointTypes);
    }
}
