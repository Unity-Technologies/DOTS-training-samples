//#define SYNCED_SIMULATION
#if SYNCED_SIMULATION
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

partial class PointManager : MonoBehaviour
{
    int frameCount = 1;
    double totalTimeMs = 0;
    internal void FixedUpdate()
    {
        using (var t = new DebugTimer($"Simulate ({frameCount} frames, avg. {totalTimeMs / frameCount:F1} ms)", frameCount++ % 300 == 0 ? 8d : double.MaxValue))
        {
            Simulate();
            totalTimeMs += t.timeMs;
        }
    }
    
    void Simulate()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var activeTornados = tornados.Where(t => t.simulate).ToArray();
        var random = new Unity.Mathematics.Random(1234);

        float invDamping = 1f - damping;
        using (var pointQuery = em.CreateEntityQuery(typeof(DynamicPoint), typeof(Point)))
        using (var points = pointQuery.ToEntityArray(Allocator.TempJob))
        {
            for (int i = 0; i < points.Length; i++)
            {
                var point = em.GetComponentData<Point>(points[i]);
                var start = point.pos;
                point.old.y += .01f;

                foreach (var tornado in activeTornados)
                {
                    // tornado force
                    float tdx = tornado.x + Tornado.Sway(point.pos.y) - point.pos.x;
                    float tdz = tornado.y - point.pos.z;
                    float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
                    tdx /= tornadoDist;
                    tdz /= tornadoDist;
                    if (tornadoDist < tornado.maxForceDist)
                    {
                        float force = (1f - tornadoDist / tornado.maxForceDist);
                        float yFader = Mathf.Clamp01(1f - point.pos.y / tornado.height);
                        force *= tornado.fader * tornado.force * random.NextFloat(-.3f, 1.3f);
                        float forceY = tornado.upForce;
                        point.old.y -= forceY * force;
                        float forceX = -tdz + tdx * tornado.inwardForce * yFader;
                        float forceZ = tdx + tdz * tornado.inwardForce * yFader;
                        point.old.x -= forceX * force;
                        point.old.z -= forceZ * force;

                        // TODO: ecb create affected beam
                    }
                }

                point.pos += (point.pos - point.old) * invDamping;
                point.old = start;

                if (point.pos.y < 0f)
                {
                    point.pos.y = 0f;
                    point.old.y = -point.old.y;
                    point.old.x += (point.pos.x - point.pos.x) * friction;
                    point.old.z += (point.pos.z - point.pos.z) * friction;
                }

                // Write back point data
                em.SetComponentData(points[i], point);
            }
        }

        using (var bq = em.CreateEntityQuery(typeof(Beam)))
        using (var beams = bq.ToEntityArray(Allocator.TempJob))
        {
            for (int i = 0; i < beams.Length; ++i)
            {
                var beam = em.GetComponentData<Beam>(beams[i]);
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
                var writeBackBeam = false;

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

                var matrix = matrices[beam.m1i][beam.m2i];
                var translate = (point1.pos + point2.pos) * .5f;
                if (norm.x * beam.norm.x + norm.y * beam.norm.y + norm.z * beam.norm.z < .99f)
                {
                    // bar has rotated: expensive full-matrix computation
                    matrix = Matrix4x4.TRS(translate, Quaternion.LookRotation(delta), new Vector3(beam.thickness, beam.thickness, dist));
                    beam.norm = norm;
                }
                else
                {
                    // bar hasn't rotated: only update the position elements
                    matrix.m03 = translate.x;
                    matrix.m13 = translate.y;
                    matrix.m23 = translate.z;
                }

                matrices[beam.m1i][beam.m2i] = matrix;

                if (breaks)
                {
                    if (point2.neighborCount > 1)
                    {
                        point2.neighborCount--;
                        writeBackPoint2 = true;

                        beam.point2 = CreatePoint(em, point2.pos, false, 1);
                        writeBackBeam = true;
                    }
                    else if (point1.neighborCount > 1)
                    {
                        point1.neighborCount--;
                        writeBackPoint1 = true;

                        beam.point1 = CreatePoint(em, point1.pos, false, 1);
                        writeBackBeam = true;
                    }
                }

                if (writeBackPoint1) em.SetComponentData(beam.point1, point1);
                if (writeBackPoint2) em.SetComponentData(beam.point2, point2);
                if (writeBackBeam) em.SetComponentData(beams[i], beam);
            }
        }
    }
}

#endif
