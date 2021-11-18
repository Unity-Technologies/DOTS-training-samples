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

    private Allocator m_ECBAllocator;

    public const double showProfileTrackerThreshold = 10d;

    public BaseTornadoSystem()
        : this(Allocator.Temp)
    {
    }

    public BaseTornadoSystem(Allocator ecbAllocator)
        : base()
    {
        m_ECBAllocator = ecbAllocator;
    }

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
        using (var ecb = new EntityCommandBuffer(m_ECBAllocator))
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
        var j1 = Entities.WithNone<AnchoredPoint>().ForEach((ref Point point, ref PointStart ps) =>
        {
            ps.start = point.pos;
            point.old.y += .01f;
        }).ScheduleParallel(Dependency);

        // Compute distance
        using (var ecb = new EntityCommandBuffer(Allocator.TempJob))
        {
            var cmd = ecb.AsParallelWriter();
            var j2 = Entities.WithNone<AnchoredPoint>().WithReadOnly(tornados).ForEach((in Entity e, in int entityInQueryIndex, in Point point) =>
            {
                // Apply tornado force
                foreach (var t in tornados)
                {
                    var sway = math.sin(point.pos.y / 5f + time / 4f) * 3f;
                    var td = new float2(t.x + sway - point.pos.x, t.y - point.pos.z);
                    var tornadoDist = math.length(td);
                    if (tornadoDist < t.data.maxForceDist)
                        cmd.SetComponent(entityInQueryIndex, e, new AffectedPoint(t, tornadoDist, td / tornadoDist));
                }
            }).ScheduleParallel(JobHandle.CombineDependencies(setupJob, j1));

            j2.Complete();
            ecb.Playback(EntityManager);
        }
    }
}

partial class Tornado0BindMatrices : BaseTornadoSystem
{
    private bool m_CaptureMatrices = true;

    protected override void OnUpdate(EntityCommandBuffer _)
    {
        if (!m_CaptureMatrices)
            return;
        m_CaptureMatrices = false;

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
    }
}

[UpdateAfter(typeof(Tornado0SetupSystem))]
partial class Tornado1ApplyForcesSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Unity.Mathematics.Random((uint)Time.ElapsedTime + 1);
        var j1 = Entities.ForEach((ref Point point, ref AffectedPoint a) =>
        {
            if (!a.enabled)
                return;

            var yFader = math.saturate(1f - point.pos.y / a.height) * a.inwardForce;
            var fv = new float3(-a.tdir.y + a.tdir.x * yFader, a.upForce, a.tdir.x + a.tdir.y * yFader);
            var force = (1f - a.distance / a.maxForceDist) * (a.fader * a.force * random.NextFloat(-.3f, 1.3f));

            point.old -= fv * force;
            a.enabled = false;
        }).ScheduleParallel(Dependency);

        Dependency = Entities.ForEach((ref Point point, in PointStart ps, in PointDamping d) =>
        {
            point.pos += (point.pos - point.old) * d.invDamping;
            point.old = ps.start;
            if (point.pos.y < 0f)
            {
                point.pos.y = 0f;
                point.old += (point.pos.x - point.old.x) * d.friction;
                point.old.y = -point.old.y;
            }
        }).ScheduleParallel(j1);
    }
}

[UpdateAfter(typeof(Tornado1ApplyForcesSystem))]
partial class Tornado2ApplyBeamForcesSystem : BaseTornadoSystem
{
    struct CachedPoint
    {
        public float3 pos;
        public float3 old;
        public int neighbors;
        public bool anchored;
    }

    EntityQuery m_PointQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_PointQuery = EntityManager.CreateEntityQuery(typeof(Point));
    }

    protected override void OnDestroy()
    {
        m_PointQuery.Dispose();
        base.OnDestroy();
    }

    protected override void OnUpdate(EntityCommandBuffer ecb)
    {
        var em = EntityManager;
        var breakResistance = m_BreakResistance;

        var pointCache = new NativeHashMap<int, CachedPoint>(m_PointQuery.CalculateEntityCount(), Allocator.TempJob);
        var pcw = pointCache.AsParallelWriter();
        var j1 = Entities.WithAll<DynamicPoint>().ForEach((in Entity entity, in Point p, in PointNeighbors pn) =>
        {
            var k = entity.GetHashCode();
            pcw.TryAdd(k, new CachedPoint 
            { 
                anchored = false,
                pos = p.pos,
                old = p.old,
                neighbors = pn.neighborCount
            });
        }).ScheduleParallel(Dependency);

        var j2 = Entities.WithAll<AnchoredPoint>().ForEach((in Entity entity, in Point p, in PointNeighbors pn) =>
        {
            var k = entity.GetHashCode();
            pcw.TryAdd(k, new CachedPoint
            {
                anchored = true,
                pos = p.pos,
                old = p.old,
                neighbors = pn.neighborCount
            });
        }).ScheduleParallel(j1);

        j2.Complete();

        //UnityEngine.Debug.Log($"{pointCache.Count()} points");

        Entities.ForEach((in Entity entity, in Beam beam) =>
        {
            var k1 = beam.point1.GetHashCode();
            var k2 = beam.point2.GetHashCode();

            //UnityEngine.Debug.Log($"{k1},{k2} points");

            var c1 = pointCache[k1];
            var c2 = pointCache[k2];

            var point1Anchored = c1.anchored;
            var point2Anchored = c2.anchored;

            var delta = c2.pos - c1.pos;
            var dist = math.length(delta);
            var extraDist = dist - beam.length;
            var norm = delta / dist;
            var push = norm * extraDist * .5f;
            var breaks = math.abs(extraDist) > breakResistance;
            var writeBackPoint1 = false;
            var writeBackPoint2 = false;

            if (!point1Anchored && !point2Anchored)
            {
                c1.pos += push;
                c2.pos -= push;
                writeBackPoint1 = writeBackPoint2 = true;
            }
            else if (point1Anchored)
            {
                c2.pos -= push * 2f;
                writeBackPoint2 = true;
            }
            else if (point2Anchored)
            {
                c2.pos += push * 2f;
                writeBackPoint1 = true;
            }

            if (writeBackPoint1) 
            {
                em.SetComponentData(beam.point1, new Point(c1.pos, c1.old));
                pointCache[k1] = c1;
            }
            if (writeBackPoint2)
            {
                em.SetComponentData(beam.point2, new Point(c2.pos, c2.old));
                pointCache[k2] = c2;
            }

            int pi = 0;
            if (breaks)
            {
                if (c2.neighbors > 1)
                {
                    --c2.neighbors;
                    writeBackPoint2 = true;
                    em.SetComponentData(beam.point2, new PointNeighbors(c2.neighbors));
                    pi = 2;
                }
                else
                {
                    if (c1.neighbors > 1)
                    {
                        --c1.neighbors;
                        writeBackPoint1 = true;
                        em.SetComponentData(beam.point1, new PointNeighbors(c1.neighbors));
                        pi = 1;
                    }
                }
            }

            if (writeBackPoint1 || writeBackPoint2)
                ecb.SetComponent(entity, new BeamModif(c1.pos, c2.pos, norm, delta, dist, breaks, pi));
            //else
              //  UnityEngine.Debug.Log("test");
        }).Run();

        pointCache.Dispose();
    }
}

[UpdateAfter(typeof(Tornado0BindMatrices))]
[UpdateAfter(typeof(Tornado2ApplyBeamForcesSystem))]
partial class Tornado3UpdateBeamsSystem : BaseTornadoSystem
{
    public Tornado3UpdateBeamsSystem()
        : base(Allocator.TempJob)
    {
    }

    protected override void OnUpdate(EntityCommandBuffer ecb)
    {
        var damping = m_Damping;
        var friction = m_Friction;
        var FixedPointArchType = ArcheTypes.FixedPoint;
        var DynamicPointArchType = ArcheTypes.DynamicPoint;

        var cmd = ecb.AsParallelWriter();
        Entities.ForEach((ref BeamModif bm, in Entity entity, in int entityInQueryIndex, in Beam _beam) =>
        {
            if (!bm.enabled)
                return;

            var beam = _beam;
            var writeBackBeam = false; // Used to serialize beam modifications

            var translate = (bm.p1 + bm.p2) * .5f;
            var dd = bm.norm.x * beam.norm.x + bm.norm.y * beam.norm.y + bm.norm.z * beam.norm.z;
            if (dd < .99f)
            {
                unsafe
                {
                    // Bar has rotated: expensive full-matrix computation
                    *beam.matrix = Matrix4x4.TRS(translate, Quaternion.LookRotation(bm.delta), new Vector3(beam.thickness, beam.thickness, bm.dist));
                }
                beam.norm = bm.norm;
                writeBackBeam = true;
            }
            else
            {
                unsafe
                {
                    // Bar hasn't rotated: only update the position elements
                    (*beam.matrix).m03 = translate.x;
                    (*beam.matrix).m13 = translate.y;
                    (*beam.matrix).m23 = translate.z;
                }
            }

            if (bm.breaks)
            {
                if (bm.pi == 2)
                {
                    beam.point2 = CreatePoint(cmd, entityInQueryIndex, bm.p2, false, 1, damping, friction, FixedPointArchType, DynamicPointArchType);
                    writeBackBeam = true;
                }
                else if (bm.pi == 1)
                {
                    beam.point1 = CreatePoint(cmd, entityInQueryIndex, bm.p1, false, 1, damping, friction, FixedPointArchType, DynamicPointArchType);
                    writeBackBeam = true;
                }
            }

            bm.enabled = false;
            if (writeBackBeam)
                cmd.SetComponent(entityInQueryIndex, entity, beam);
        }).ScheduleParallel(Dependency).Complete();
    }

    static Entity CreatePoint(EntityCommandBuffer.ParallelWriter ecb, in int sortKey, in float3 pos, bool anchored, int neighborCount, float damping, float friction,
                                in EntityArchetype fixedPointArchType, in EntityArchetype dynamicPointArchType)
    {
        var point = ecb.CreateEntity(sortKey, anchored ? fixedPointArchType : dynamicPointArchType);
        ecb.SetComponent(sortKey, point, new Point(pos));
        ecb.SetComponent(sortKey, point, new PointNeighbors(neighborCount));
        ecb.SetComponent(sortKey, point, new PointDamping(1f - damping, friction));
        return point;
    }
}

#endif