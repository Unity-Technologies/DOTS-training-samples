using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    public partial class AnchorSimulatorSystem : SystemBase
    {
        float m_TornadoFader = 0;

        protected override void OnUpdate()
        {
            var random = new Random(1234);

            var deltaTime = Time.DeltaTime;
            var elapsedTime = Time.ElapsedTime;

            var sme = GetSingletonEntity<SimulationManager>();
            var sm = GetComponent<SimulationManager>(sme);

            float expForce = sm.expForce;
            float breakResistance = sm.breakResistance;
            float damping = sm.damping;
            float friction = sm.friction;

            m_TornadoFader = Mathf.Clamp01(m_TornadoFader + deltaTime / 10f);
            var faderCopy = m_TornadoFader;

            EntityQuery query = GetEntityQuery(
                ComponentType.ReadOnly<Tornado>(),
                ComponentType.ReadOnly<Simulated>()
            );

            var tornados = query.ToComponentDataArray<Tornado>(Allocator.Persistent);
            if (tornados.Length == 0)
                return;

            var tornadoJob = Entities
                .WithNone<FixedAnchor>()
                .WithReadOnly(tornados)
                .ForEach((ref Anchor point) =>
                {
                    for (var i = 0; i < tornados.Length; ++i)
                    {
                        var tornado = tornados[i];
                        float invDamping = 1f - damping;

                        float startX = point.position.x;
                        float startY = point.position.y;
                        float startZ = point.position.z;

                        point.oldPosition.y += .01f;

                        // tornado force
                        float tdx = tornado.position.x + PointManager.TornadoSway(startY, (float)elapsedTime) - startX;
                        float tdz = tornado.position.z - startZ;
                        float tornadoDist = math.sqrt(tdx * tdx + tdz * tdz);
                        tdx /= tornadoDist;
                        tdz /= tornadoDist;
                        if (tornadoDist < tornado.maxForceDist)
                        {
                            float force = (1f - tornadoDist / tornado.maxForceDist);
                            float yFader = Mathf.Clamp01(1f - startY / tornado.height);
                            force *= faderCopy * tornado.force * random.NextFloat(-.3f, 1.3f);
                            float forceY = tornado.upForce;
                            point.oldPosition.y -= forceY * force;
                            float forceX = -tdz + tdx * tornado.inwardForce * yFader;
                            float forceZ = tdx + tdz * tornado.inwardForce * yFader;
                            point.oldPosition.x -= forceX * force;
                            point.oldPosition.z -= forceZ * force;
                        }

                        point.position.x += (point.position.x - point.oldPosition.x) * invDamping;
                        point.position.y += (point.position.y - point.oldPosition.y) * invDamping;
                        point.position.z += (point.position.z - point.oldPosition.z) * invDamping;

                        point.oldPosition.x = startX;
                        point.oldPosition.y = startY;
                        point.oldPosition.z = startZ;
                        if (point.position.y < 0f)
                        {
                            point.position.y = 0f;
                            point.oldPosition.y = -point.oldPosition.y;
                            point.oldPosition.x += (point.position.x - point.oldPosition.x) * friction;
                            point.oldPosition.z += (point.position.z - point.oldPosition.z) * friction;
                        }
                    }

                }).ScheduleParallel(this.Dependency);

            var beamConstraintsJob = Entities.ForEach((ref Beam beam) =>
            {
                var point1 = beam.p1;
                var point2 = beam.p2;

                var a1 = GetComponent<Anchor>(point1);
                var a2 = GetComponent<Anchor>(point2);

                float dx = a2.position.x - a1.position.x;
                float dy = a2.position.y - a1.position.y;
                float dz = a2.position.z - a1.position.z;
                beam.newD = new float3(dx, dy, dz);

                float dist = math.sqrt(dx * dx + dy * dy + dz * dz);
                float extraDist = dist - beam.length;

                float pushX = (dx / dist * extraDist) * .5f;
                float pushY = (dy / dist * extraDist) * .5f;
                float pushZ = (dz / dist * extraDist) * .5f;

                var p1Anchored = HasComponent<FixedAnchor>(point1);
                var p2Anchored = HasComponent<FixedAnchor>(point2);
                if (!p1Anchored && !p2Anchored)
                {
                    a1.position.x += pushX;
                    a1.position.y += pushY;
                    a1.position.z += pushZ;
                    a2.position.x -= pushX;
                    a2.position.y -= pushY;
                    a2.position.z -= pushZ;
                }
                else if (p1Anchored)
                {
                    a2.position.x -= pushX * 2f;
                    a2.position.y -= pushY * 2f;
                    a2.position.z -= pushZ * 2f;
                }
                else
                {
                    a1.position.x += pushX * 2f;
                    a1.position.y += pushY * 2f;
                    a1.position.z += pushZ * 2f;
                }
            }).Schedule(tornadoJob);

            var beamRenderingJob = Entities.ForEach((ref Translation t, ref Rotation r, ref Beam beam) =>
            {
                var point1 = beam.p1;
                var point2 = beam.p2;

                var a1 = GetComponent<Anchor>(point1);
                var a2 = GetComponent<Anchor>(point2);

                var newD = beam.newD;

                float dist = math.sqrt(newD.x * newD.x + newD.y * newD.y + newD.z * newD.z);

                t.Value.x = (a1.position.x + a2.position.x) * .5f;
                t.Value.y = (a1.position.y + a2.position.y) * .5f;
                t.Value.z = (a1.position.z + a2.position.z) * .5f;

                if (newD.x / dist * beam.oldD.x + newD.y / dist * beam.oldD.y + newD.z / dist * beam.oldD.z < .99f)
                {
                    r.Value = Quaternion.LookRotation(newD);
                    beam.oldD.x = newD.x / dist;
                    beam.oldD.y = newD.y / dist;
                    beam.oldD.z = newD.z / dist;
                }
            }).ScheduleParallel(beamConstraintsJob);

            this.Dependency = beamRenderingJob;
            tornados.Dispose(Dependency);
        }
    }
}