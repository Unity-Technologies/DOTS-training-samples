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

partial class TornadoSetupSystem_0 : SystemBase
{
    protected override void OnUpdate()
    {
        var time = (float)Time.ElapsedTime;
        var tornados = new NativeList<TornadoState>(1, Allocator.Persistent);

        Entities.WithAll<TornadoData>().ForEach((in Entity entity, in Translation translation, in TornadoData data, in TornadoFader fader) =>
        {
            tornados.Add(new TornadoState(entity, translation.Value, fader.fader, data));
        }).Run();

        // Drop
        var j1 = Entities.ForEach((ref Point point) =>
        {
            point.start = point.pos;
            point.old.y += .01f;
        }).ScheduleParallel(Dependency);

        // Compute distance
        var _ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecb = _ecb.AsParallelWriter();
        var j2 = Entities.WithReadOnly(tornados).ForEach((in Entity e, in int entityInQueryIndex, in Point point) =>
        {
            // Apply tornado force
            foreach (var t in tornados)
            {
                float sway = math.sin(point.pos.y / 5f + time / 4f) * 3f;
                float tdx = t.x + sway - point.pos.x;
                float tdz = t.y - point.pos.z;
                float tornadoDist = math.sqrt(tdx * tdx + tdz * tdz);
                tdx /= tornadoDist;
                tdz /= tornadoDist;

                if (tornadoDist < t.data.maxForceDist)
                    ecb.AddComponent(entityInQueryIndex, e, new AffectedPoint(t.entity, tornadoDist, new float2(tdx, tdz)));
            }
        }).ScheduleParallel(j1);
        
        tornados.Dispose(j2);
        j2.Complete();

        _ecb.Playback(EntityManager);
        _ecb.Dispose();
    }
}

[UpdateAfter(typeof(TornadoSetupSystem_0))]
partial class TornadoApplyForcesSystem_1 : SystemBase
{
    protected override void OnUpdate()
    {
        var em = EntityManager;
        var random = new Unity.Mathematics.Random(1234);

        using (var ecb = new EntityCommandBuffer(Allocator.Temp))
        {
            Entities.ForEach((Entity entity, ref Point point, in AffectedPoint a) =>
            {
                var tornado = em.GetComponentData<TornadoData>(a.tornado);
                var fader = em.GetComponentData<TornadoFader>(a.tornado);
                float force = (1f - a.distance / tornado.maxForceDist);
                float yFader = Mathf.Clamp01(1f - point.pos.y / tornado.height);
                force *= fader.fader * tornado.force * random.NextFloat(-.3f, 1.3f);
                float forceY = tornado.upForce;
                point.old.y -= forceY * force;
                float forceX = -a.tdir.y + a.tdir.x * tornado.inwardForce * yFader;
                float forceZ = a.tdir.x + a.tdir.y * tornado.inwardForce * yFader;
                point.old.x -= forceX * force;
                point.old.z -= forceZ * force;

                ecb.RemoveComponent<AffectedPoint>(entity);
            }).Run();

            ecb.Playback(EntityManager);
        }

        Entities.ForEach((ref Point point, in PointDamping d) =>
        {
            point.pos += (point.pos - point.old) * d.invDamping;
            point.old = point.start;

            if (point.pos.y < 0f)
            {
                point.pos.y = 0f;
                point.old.y = -point.old.y;
                point.old.x += (point.pos.x - point.pos.x) * d.friction;
                point.old.z += (point.pos.z - point.pos.z) * d.friction;
            }
        }).Run();
    }
}

[UpdateAfter(typeof(TornadoApplyForcesSystem_1))]
partial class TornadoApplyBeamForcesSystem_2 : BaseTornadoSystem
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
            var breaks = Mathf.Abs(extraDist) > breakResistance;
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

[UpdateAfter(typeof(TornadoApplyBeamForcesSystem_2))]
partial class TornadoUpdateBeamsSystem_3 : BaseTornadoSystem
{
    protected override void OnUpdate(EntityCommandBuffer ecb)
    {
        var damping = m_Damping;
        var friction = m_Friction;
        var matrices = m_Matrices;

        Entities.WithoutBurst().ForEach((in Entity entity, in Beam beam, in BeamModif bm) =>
        {
            UpdateBeams(ecb, entity, beam, matrices, bm, damping, friction);
        }).Run();
    }

    static void UpdateBeams(in EntityCommandBuffer ecb, in Entity entity, in Beam _beam, Matrix4x4[][] matrices, in BeamModif bm, in float damping, in float friction)
    {
        ecb.RemoveComponent<BeamModif>(entity);

        var beam = _beam;
        var matrix = matrices[beam.m1i][beam.m2i];
        var translate = (bm.p1 + bm.p2) * .5f;
        var dd = bm.norm.x * beam.norm.x + bm.norm.y * beam.norm.y + bm.norm.z * beam.norm.z;
        var writeBackBeam = false;

        if (dd < .99f)
        {
            // bar has rotated: expensive full-matrix computation
            matrix = Matrix4x4.TRS(translate, Quaternion.LookRotation(bm.delta), new Vector3(beam.thickness, beam.thickness, bm.dist));
            beam.norm = bm.norm;
            writeBackBeam = true;
        }
        else
        {
            // bar hasn't rotated: only update the position elements
            matrix.m03 = translate.x;
            matrix.m13 = translate.y;
            matrix.m23 = translate.z;
        }

        matrices[beam.m1i][beam.m2i] = matrix;

        if (bm.breaks)
        {
            if (bm.pi == 2)
            {
                beam.point2 = CreatePoint(ecb, bm.p2, false, 1, damping, friction);
                writeBackBeam = true;
            }
            else if (bm.pi == 1)
            {
                beam.point1 = CreatePoint(ecb, bm.p1, false, 1, damping, friction);
                writeBackBeam = true;
            }
        }

        if (writeBackBeam)
            ecb.SetComponent(entity, beam);
    }

    static Entity CreatePoint(EntityCommandBuffer ecb, in float3 pos, bool anchored, int neighborCount, float damping, float friction)
    {
        var point = ecb.CreateEntity();
        ecb.AddComponent(point, new Point(pos) { neighborCount = neighborCount });
        ecb.AddComponent(point, new PointDamping(1f - damping, friction));
        if (anchored)
            ecb.AddComponent<AnchoredPoint>(point);
        else
            ecb.AddComponent<DynamicPoint>(point);
        return point;
    }
}

#endif