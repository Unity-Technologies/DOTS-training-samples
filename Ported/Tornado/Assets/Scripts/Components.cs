using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct Point : IComponentData
{
    public float3 pos;
    public float3 old;
    public Point(in float3 pos)
    {
        old = this.pos = pos;
    }

    public Point(in float3 pos, in float3 old)
    {
        this.pos = pos;
        this.old = old;
    }
}

struct PointNeighbors : IComponentData
{
    public int neighborCount;
    public PointNeighbors(in int neighborCount)
    {
        this.neighborCount = neighborCount;
    }
}

struct PointStart : IComponentData
{
    public float3 start;

    public PointStart(in float3 pos)
    {
        start = pos;
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

/*
struct BeamModifMask : IComponentData
{
    public bool enabled;
    public BeamModifMask(bool enabled)
    {
        this.enabled = enabled;
    }
}*/

struct BeamModif : IComponentData
{
    public bool enabled;
    public readonly float3 p1;
    public readonly float3 p2;
    public readonly float3 norm;
    public readonly float3 delta;
    public readonly float dist;
    public readonly bool breaks;
    public readonly int pi;

    public BeamModif(in float3 p1, in float3 p2, in float3 norm, in float3 delta, float dist, bool breaks, int pi)
    {
        enabled = true;
        this.p1 = p1;
        this.p2 = p2;
        this.norm = norm;
        this.delta = delta;
        this.dist = dist;
        this.breaks = breaks;
        this.pi = pi;
    }
}

static class ArcheTypes
{
    public static readonly ComponentType[] FixedPointTypes = new ComponentType[] { typeof(Point), typeof(PointDamping), typeof(AnchoredPoint), typeof(PointNeighbors) };
    public static readonly ComponentType[] DynamicPointTypes = new ComponentType[] { typeof(Point), typeof(PointDamping), typeof(DynamicPoint), typeof(AffectedPoint), typeof(PointStart), typeof(PointNeighbors) };
    public static readonly ComponentType[] BeamTypes = new ComponentType[] { typeof(Beam), /*typeof(BeamModifMask),*/ typeof(BeamModif) };

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
