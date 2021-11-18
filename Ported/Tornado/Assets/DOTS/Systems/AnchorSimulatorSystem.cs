using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AnchorSimulatorSystem : SystemBase
    {
        float m_TornadoFader = 0;

        BuildingSpawnerSystem m_BuildingSpawnerSystem;

        protected override void OnCreate()
        {
            m_BuildingSpawnerSystem = World.GetExistingSystem<BuildingSpawnerSystem>();
        }

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
            float invDamping = 1f - damping;

            m_TornadoFader = Mathf.Clamp01(m_TornadoFader + deltaTime / 10f);
            var faderCopy = m_TornadoFader;

            EntityQuery query = GetEntityQuery(
                ComponentType.ReadOnly<Tornado>(),
                ComponentType.ReadOnly<Simulated>()
            );

            var tornados = query.ToComponentDataArray<Tornado>(Allocator.Persistent);

            var tornadoJob = Entities
                .WithName("TornadoForces")
                .WithNone<FixedAnchor>()
                .WithReadOnly(tornados)
                .ForEach((ref Anchor point) =>
                {
                    float startX = point.position.x;
                    float startY = point.position.y;
                    float startZ = point.position.z;

                    for (var i = 0; i < tornados.Length; ++i)
                    {
                        var tornado = tornados[i];

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
                    }

                }).ScheduleParallel(this.Dependency);

            var applyForcesJob = Entities
                .WithName("ApplyForces")
                .WithNone<FixedAnchor>()
                .ForEach((ref Anchor point) =>
                {
                    float startX = point.position.x;
                    float startY = point.position.y;
                    float startZ = point.position.z;

                    point.oldPosition.y += .01f;

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
                }).ScheduleParallel(tornadoJob);

            var beamDamping = 0.85f;
            // var localBeams = m_BuildingSpawnerSystem.beams;
            // var beamConstraintsJob = Job
            //     .WithName("BeamConstraints")
            //     .WithNativeDisableContainerSafetyRestriction(localBeams)
            //     .WithCode(() =>
            // {
            //     for (var i = 0; i < localBeams.Length; ++i)
            //     {
            //         var beam = localBeams[i];
            //
            //         var point1 = beam.p1;
            //         var point2 = beam.p2;
            //
            //         var a1 = GetComponent<Anchor>(point1);
            //         var a2 = GetComponent<Anchor>(point2);
            //
            //         float dx = a2.position.x - a1.position.x;
            //         float dy = a2.position.y - a1.position.y;
            //         float dz = a2.position.z - a1.position.z;
            //         beam.newD = new float3(dx, dy, dz);
            //         localBeams[i] = beam;
            //
            //         float dist = math.sqrt(dx * dx + dy * dy + dz * dz);
            //         float extraDist = dist - beam.length;
            //
            //         float pushX = (dx / dist * extraDist) * .5f * beamDamping;
            //         float pushY = (dy / dist * extraDist) * .5f * beamDamping;
            //         float pushZ = (dz / dist * extraDist) * .5f * beamDamping;
            //
            //         var p1Anchored = HasComponent<FixedAnchor>(point1);
            //         var p2Anchored = HasComponent<FixedAnchor>(point2);
            //         if (!p1Anchored && !p2Anchored)
            //         {
            //             a1.position.x += pushX;
            //             a1.position.y += pushY;
            //             a1.position.z += pushZ;
            //             a2.position.x -= pushX;
            //             a2.position.y -= pushY;
            //             a2.position.z -= pushZ;
            //         }
            //         else if (p1Anchored)
            //         {
            //             a2.position.x -= pushX * 2f;
            //             a2.position.y -= pushY * 2f;
            //             a2.position.z -= pushZ * 2f;
            //         }
            //         else
            //         {
            //             a1.position.x += pushX * 2f;
            //             a1.position.y += pushY * 2f;
            //             a1.position.z += pushZ * 2f;
            //         }
            //
            //         SetComponent(point1, a1);
            //         SetComponent(point2, a2);
            //     }
            // }).Schedule(applyForcesJob);

            var localBeams = m_BuildingSpawnerSystem.beams;
            var componentDataFromEntity = GetComponentDataFromEntity<Anchor>();
            var beamConstraintsJob = Entities
                .WithName("BeamConstraints")
                .WithNativeDisableContainerSafetyRestriction(localBeams)
                .WithNativeDisableContainerSafetyRestriction(componentDataFromEntity)
                .ForEach((int entityInQueryIndex, ref DynamicBuffer<BeamBufferElement> beamEntityBuffer) =>
            {
                for (var i = 0; i < beamEntityBuffer.Length; ++i)
                {
                    var beamElement = beamEntityBuffer[i];
                    var beamIndex = beamElement.Value;
                    var beam = localBeams[beamIndex];
            
                    var point1 = beam.p1;
                    var point2 = beam.p2;
            
                    var a1 = componentDataFromEntity[point1];
                    var a2 = componentDataFromEntity[point2];
            
                    float dx = a2.position.x - a1.position.x;
                    float dy = a2.position.y - a1.position.y;
                    float dz = a2.position.z - a1.position.z;
                    beam.newD = new float3(dx, dy, dz);
                    
            
                    float dist = math.sqrt(dx * dx + dy * dy + dz * dz);
                    float extraDist = dist - beam.length;
            
                    float pushX = (dx / dist * extraDist) * .5f * beamDamping;
                    float pushY = (dy / dist * extraDist) * .5f * beamDamping;
                    float pushZ = (dz / dist * extraDist) * .5f * beamDamping;
            
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
            
                    componentDataFromEntity[point1] = a1;
                    componentDataFromEntity[point2] = a2;

                    // if (Mathf.Abs(extraDist) > breakResistance)
                    // {
                    //     if (point2.neighborCount > 1)
                    //     {
                    //         point2.neighborCount--;
                    //         Point newPoint = new Point();
                    //         newPoint.CopyFrom(point2);
                    //         newPoint.neighborCount = 1;
                    //         points[pointCount] = newPoint;
                    //         bar.point2 = newPoint;
                    //         pointCount++;
                    //     }
                    //     else if (point1.neighborCount > 1)
                    //     {
                    //         point1.neighborCount--;
                    //         Point newPoint = new Point();
                    //         newPoint.CopyFrom(point1);
                    //         newPoint.neighborCount = 1;
                    //         points[pointCount] = newPoint;
                    //         bar.point1 = newPoint;
                    //         pointCount++;
                    //     }
                    // }

                    localBeams[beamIndex] = beam;
                }
            }).ScheduleParallel(applyForcesJob);

            this.Dependency = beamConstraintsJob;
            tornados.Dispose(Dependency);
        }
    }
}