#define DOTS_SIMULATION
#if DOTS_SIMULATION
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

readonly struct TornadoState
{
    public readonly Entity entity;
    public readonly float3 position;
    public readonly float fader;
    public readonly TornadoData data;

    public TornadoState(in Entity entity, in float3 position, in float fader, in TornadoData data)
    {
        this.entity = entity;
        this.position = position;
        this.fader = fader;
        this.data = data;
    }

    public float x => position.x;
    public float y => position.z;
}

abstract partial class BaseTornadoSystem : SystemBase
{
    private PointManager m_PointManager;
    protected float m_Damping;
    protected float m_Friction;
    protected float m_BreakResistance;  
    protected Matrix4x4[][] m_Matrices;

    public const double showProfileTrackerThreshold = 10d;

    private void Initialize()
    {
        m_PointManager = GameObject.FindObjectOfType<PointManager>();
        m_Damping = m_PointManager.damping;
        m_Friction = m_PointManager.friction;
        m_BreakResistance = m_PointManager.breakResistance;
        m_Matrices = m_PointManager.matrices;
    }

    protected override void OnUpdate()
    {
        if (m_PointManager == null)
            Initialize();
        using (var ecb = new EntityCommandBuffer(Allocator.Temp))
        {
            OnUpdate(ecb);
            ecb.Playback(EntityManager);
        }
    }
    protected abstract void OnUpdate(EntityCommandBuffer ecb);
}

partial class Tornado0SetupSystem : SystemBase
{
    private NativeList<TornadoState> m_Tornados;

    protected override void OnCreate()
    {
        m_Tornados = new NativeList<TornadoState>(1, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        m_Tornados.Dispose();
    }

    protected override void OnUpdate()
    {
        var time = (float)Time.ElapsedTime;

        m_Tornados.Clear();
        var tornados = m_Tornados;

        var setupJob = Entities.WithAll<TornadoData>().ForEach((in Entity entity, in Translation translation, in TornadoData data, in TornadoFader fader) =>
        {
            tornados.Add(new TornadoState(entity, translation.Value, fader.value, data));
        }).Schedule(Dependency);

        // Drop
        var j1 = Entities.ForEach((ref Point point) =>
        {
            point.start = point.pos;
            point.old.y += .01f;
        }).ScheduleParallel(Dependency);

        // Compute distance
        using (var ecb = new EntityCommandBuffer(Allocator.TempJob))
        {
            var cmd = ecb.AsParallelWriter();
            var j2 = Entities.WithReadOnly(tornados).ForEach((in Entity e, in int entityInQueryIndex, in Point point) =>
            {
                // Apply tornado force
                foreach (var t in tornados)
                {
                    var sway = math.sin(point.pos.y / 5f + time / 4f) * 3f;
                    var td = new float2(t.x + sway - point.pos.x, t.y - point.pos.z);
                    var tornadoDist = math.length(td);
                    if (tornadoDist < t.data.maxForceDist)
                        cmd.AddComponent(entityInQueryIndex, e, new AffectedPoint(t, tornadoDist, td / tornadoDist));
                }
            }).ScheduleParallel(JobHandle.CombineDependencies(setupJob, j1));

            j2.Complete();

            ecb.Playback(EntityManager);
        }
    }
}

[UpdateAfter(typeof(Tornado0SetupSystem))]
partial class Tornado1ApplyForcesSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var em = EntityManager;
        var random = new Unity.Mathematics.Random(1234);

        using (var ecb = new EntityCommandBuffer(Allocator.TempJob))
        {
            var cmd = ecb.AsParallelWriter();
            var j1 = Entities.ForEach((ref Point point, in Entity entity, in int entityInQueryIndex, in AffectedPoint a) =>
            {
                cmd.RemoveComponent<AffectedPoint>(entityInQueryIndex, entity);

                var yFader = math.saturate(1f - point.pos.y / a.height) * a.inwardForce;
                var fv = new float3(-a.tdir.y + a.tdir.x * yFader, a.upForce, a.tdir.x + a.tdir.y * yFader);
                var force = (1f - a.distance / a.maxForceDist) * (a.fader * a.force * random.NextFloat(-.3f, 1.3f));

                point.old -= fv * force;
            }).WithName("tornado_apply_force").ScheduleParallel(Dependency);

            Dependency = Entities.ForEach((ref Point point, in PointDamping d) =>
            {
                point.pos += (point.pos - point.old) * d.invDamping;
                point.old = point.start;
                if (point.pos.y < 0f)
                {
                    point.pos.y = 0f;
                    point.old += (point.pos.x - point.old.x) * d.friction;
                    point.old.y = -point.old.y;
                }
            }).ScheduleParallel(j1);

            j1.Complete();
            ecb.Playback(EntityManager);
        }
    }
}

[UpdateAfter(typeof(Tornado1ApplyForcesSystem))]
partial class Tornado2ApplyBeamForcesSystem : BaseTornadoSystem
{
    protected override void OnUpdate(EntityCommandBuffer ecb)
    {
        var em = EntityManager;
        var breakResistance = m_BreakResistance;

        Entities.ForEach((in Entity entity, in Beam beam) =>
        {
            var point1 = em.GetComponentData<Point>(beam.point1);
            var point2 = em.GetComponentData<Point>(beam.point2);

            var point1Anchored = em.HasComponent<AnchoredPoint>(beam.point1);
            var point2Anchored = em.HasComponent<AnchoredPoint>(beam.point2);

            var delta = point2.pos - point1.pos; // TODO: precompute delta and length
            var dist = math.length(delta);
            var extraDist = dist - beam.length;
            var norm = delta / dist;
            var push = norm * extraDist * .5f;
            var breaks = math.abs(extraDist) > breakResistance;
            var writeBackPoint1 = false;
            var writeBackPoint2 = false;

            if (!point1Anchored && !point2Anchored)
            {
                point1.pos += push;
                point2.pos -= push;
                writeBackPoint1 = writeBackPoint2 = true;
            }
            else if (point1Anchored)
            {
                point2.pos -= push * 2f;
                writeBackPoint2 = true;
            }
            else if (point2Anchored)
            {
                point1.pos += push * 2f;
                writeBackPoint1 = true;
            }

            int pi = 0;
            if (breaks)
            {
                if (point2.neighborCount > 1)
                {
                    point2.neighborCount--;
                    writeBackPoint2 = true;
                    pi = 2;
                }
                else if (point1.neighborCount > 1)
                {
                    point1.neighborCount--;
                    writeBackPoint1 = true;
                    pi = 1;
                }
            }

            if (writeBackPoint1) em.SetComponentData(beam.point1, point1);
            if (writeBackPoint2) em.SetComponentData(beam.point2, point2);

            ecb.AddComponent(entity, new BeamModif { p1 = point1.pos, p2 = point2.pos, delta = delta, dist = dist, norm = norm, breaks = breaks, pi = pi });
        }).Run();
    }
}

[UpdateAfter(typeof(Tornado2ApplyBeamForcesSystem))]
partial class Tornado3UpdateBeamsSystem : BaseTornadoSystem
{
    private bool m_CaptureMatrices = true;

    protected override void OnUpdate(EntityCommandBuffer ecb)
    {
        if (m_CaptureMatrices)
        {
            var matrices = m_Matrices;
            Entities.WithoutBurst().ForEach((ref Beam beam) =>
            {
                var batch = matrices[beam.m1i];
                unsafe
                {
                    fixed (Matrix4x4* p = &batch[beam.m2i])
                        beam.matrix = p;
                }
            }).Run();
            m_CaptureMatrices = false;
        }

        var damping = m_Damping;
        var friction = m_Friction;
        var FixedPointArchType = PointTypes.FixedPointArchType;
        var DynamicPointArchType = PointTypes.DynamicPointArchType;

        Entities.ForEach((in Entity entity, in Beam _beam, in BeamModif bm) =>
        {
            ecb.RemoveComponent<BeamModif>(entity);

            var beam = _beam;
            var translate = (bm.p1 + bm.p2) * .5f;
            var dd = bm.norm.x * beam.norm.x + bm.norm.y * beam.norm.y + bm.norm.z * beam.norm.z;
            var writeBackBeam = false;

            if (dd < .99f)
            {
                // bar has rotated: expensive full-matrix computation
                unsafe
                {
                    *beam.matrix = Matrix4x4.TRS(translate, Quaternion.LookRotation(bm.delta), new Vector3(beam.thickness, beam.thickness, bm.dist));
                }
                beam.norm = bm.norm;
                writeBackBeam = true;
            }
            else
            {
                // bar hasn't rotated: only update the position elements
                unsafe
                {
                    (*beam.matrix).m03 = translate.x;
                    (*beam.matrix).m13 = translate.y;
                    (*beam.matrix).m23 = translate.z;
                }
            }

            if (bm.breaks)
            {
                if (bm.pi == 2)
                {
                    beam.point2 = CreatePoint(ecb, bm.p2, false, 1, damping, friction, FixedPointArchType, DynamicPointArchType);
                    writeBackBeam = true;
                }
                else if (bm.pi == 1)
                {
                    beam.point1 = CreatePoint(ecb, bm.p1, false, 1, damping, friction, FixedPointArchType, DynamicPointArchType);
                    writeBackBeam = true;
                }
            }

            if (writeBackBeam)
                ecb.SetComponent(entity, beam);
        }).Run();
    }

    static Entity CreatePoint(EntityCommandBuffer ecb, in float3 pos, bool anchored, int neighborCount, float damping, float friction,
                                in EntityArchetype fixedPointArchType, in EntityArchetype dynamicPointArchType)
    {
        var point = ecb.CreateEntity(anchored ? fixedPointArchType : dynamicPointArchType);
        ecb.SetComponent(point, new Point(pos) { neighborCount = neighborCount });
        ecb.SetComponent(point, new PointDamping(1f - damping, friction));
        return point;
    }
}

#endif