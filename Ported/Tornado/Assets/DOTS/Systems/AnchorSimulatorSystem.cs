using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
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
            if (!m_BuildingSpawnerSystem.initialized)
                return;

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

            EntityQuery query = GetEntityQuery(
                ComponentType.ReadOnly<Tornado>(),
                ComponentType.ReadOnly<Simulated>()
            );

            var tornados = query.ToComponentDataArray<Tornado>(Allocator.Persistent);

            var tornadoJob = new TornadoForcesJob()
            {
                anchors = m_BuildingSpawnerSystem.anchors,
                elapsedTime = (float)elapsedTime,
                fader = m_TornadoFader,
                random = random,
                tornados = tornados
            };
            var tornadoJobHandle = tornadoJob.Schedule(m_BuildingSpawnerSystem.pointCount.Value, 32, Dependency);

            var applyForcesJob = new ApplyForcesJob()
            {
                anchors = m_BuildingSpawnerSystem.anchors,
                friction = friction,
                invDamping = invDamping
            };
            var applyForcesJobHandle = applyForcesJob.Schedule(m_BuildingSpawnerSystem.pointCount.Value, 32, tornadoJobHandle);

            var beamDamping = 0.85f;

            var localBeams = m_BuildingSpawnerSystem.beams;
            var localAnchors = m_BuildingSpawnerSystem.anchors;
            var localPointCount = m_BuildingSpawnerSystem.pointCount;
            var beamConstraintsJob = new BeamConstraintJob()
            {
                anchors = localAnchors,
                beams = localBeams,
                beamDamping = beamDamping,
                breakResistance = breakResistance,
                connectedComponents = m_BuildingSpawnerSystem.components,
                connectedComponentsData = m_BuildingSpawnerSystem.componentsData
            };
            var beamConstraintsJobHandle = beamConstraintsJob.Schedule(m_BuildingSpawnerSystem.components.Length / 2, 1, applyForcesJobHandle);

            var breakBeamsJob = Entities
                .WithName("BreakBeams")
                .WithNativeDisableContainerSafetyRestriction(localBeams)
                .WithNativeDisableContainerSafetyRestriction(localAnchors)
                .ForEach((int entityInQueryIndex, in Beam beam) =>
                {
                    var beamData = localBeams[beam.beamDataIndex];
                    if (beamData.toBreakP2)
                    {
                        Anchor a2 = localAnchors[beamData.p2];
                        var newAnchorIndex = localPointCount.Value++;
                        beamData.p2 = newAnchorIndex;
                        Anchor newPoint = a2;
                        newPoint.neighborCount = 1;
                        beamData.toBreakP2 = false;
                        localAnchors[newAnchorIndex] = newPoint;
                    }
                    else if (beamData.toBreakP1)
                    {
                        Anchor a1 = localAnchors[beamData.p1];
                        var newAnchorIndex = localPointCount.Value++;
                        beamData.p1 = newAnchorIndex;
                        Anchor newPoint = a1;
                        newPoint.neighborCount = 1;
                        beamData.toBreakP1 = false;
                        localAnchors[newAnchorIndex] = newPoint;
                    }

                    localBeams[beam.beamDataIndex] = beamData;
                }).Schedule(beamConstraintsJobHandle);

            this.Dependency = breakBeamsJob;
            tornados.Dispose(Dependency);
        }
    }

    [BurstCompile]
    struct TornadoForcesJob : IJobParallelFor
    {
        // Jobs declare all data that will be accessed in the job
        // By declaring it as read only, multiple jobs are allowed to access the data in parallel
        [ReadOnly]
        public NativeArray<Tornado> tornados;

        // By default containers are assumed to be read & write
        public NativeArray<Anchor> anchors;

        // Delta time must be copied to the job since jobs generally don't have concept of a frame.
        // The main thread waits for the job same frame or next frame, but the job should do work deterministically
        // independent on when the job happens to run on the worker threads.
        public float elapsedTime;

        public float fader;
        public Random random;

        // The code actually running on the job
        public void Execute(int anchorId)
        {
            var point = anchors[anchorId];
            if (point.anchored)
                return;

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
                    force *= fader * tornado.force * random.NextFloat(-.3f, 1.3f);
                    float forceY = tornado.upForce;
                    point.oldPosition.y -= forceY * force;
                    float forceX = -tdz + tdx * tornado.inwardForce * yFader;
                    float forceZ = tdx + tdz * tornado.inwardForce * yFader;
                    point.oldPosition.x -= forceX * force;
                    point.oldPosition.z -= forceZ * force;
                }
            }

            anchors[anchorId] = point;
        }
    }

    [BurstCompile]
    struct ApplyForcesJob : IJobParallelFor
    {
        // By default containers are assumed to be read & write
        public NativeArray<Anchor> anchors;

        // Delta time must be copied to the job since jobs generally don't have concept of a frame.
        // The main thread waits for the job same frame or next frame, but the job should do work deterministically
        // independent on when the job happens to run on the worker threads.
        public float invDamping;
        public float friction;

        // The code actually running on the job
        public void Execute(int anchorId)
        {
            var point = anchors[anchorId];
            if (point.anchored)
                return;

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

            anchors[anchorId] = point;
        }
    }

    [BurstCompile]
    struct BeamConstraintJob : IJobParallelFor
    {
        // By default containers are assumed to be read & write
        [NativeDisableContainerSafetyRestriction]
        [NativeDisableParallelForRestriction]
        public NativeArray<Anchor> anchors;

        [NativeDisableContainerSafetyRestriction]
        [NativeDisableParallelForRestriction]
        public NativeArray<BeamData> beams;

        [ReadOnly]
        public NativeArray<int> connectedComponents;
        [ReadOnly]
        public NativeArray<int> connectedComponentsData;

        // Delta time must be copied to the job since jobs generally don't have concept of a frame.
        // The main thread waits for the job same frame or next frame, but the job should do work deterministically
        // independent on when the job happens to run on the worker threads.
        public float beamDamping;
        public float breakResistance;

        // The code actually running on the job
        public void Execute(int ccId)
        {
            var ccIndex = ccId * 2;
            var ccDataIndex = connectedComponents[ccIndex];
            var ccLength = connectedComponents[ccIndex + 1];
            for (var i = 0; i < ccLength; ++i)
            {
                var beamIndex = connectedComponentsData[ccDataIndex + i];
                var beam = beams[beamIndex];

                var point1 = beam.p1;
                var point2 = beam.p2;

                var a1 = anchors[point1];
                var a2 = anchors[point2];

                float dx = a2.position.x - a1.position.x;
                float dy = a2.position.y - a1.position.y;
                float dz = a2.position.z - a1.position.z;
                beam.newD = new float3(dx, dy, dz);


                float dist = math.sqrt(dx * dx + dy * dy + dz * dz);
                float extraDist = dist - beam.length;

                float pushX = (dx / dist * extraDist) * .5f * beamDamping;
                float pushY = (dy / dist * extraDist) * .5f * beamDamping;
                float pushZ = (dz / dist * extraDist) * .5f * beamDamping;

                var p1Anchored = a1.anchored;
                var p2Anchored = a2.anchored;
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

                if (math.abs(extraDist) > breakResistance)
                {
                    if (a2.neighborCount > 1)
                    {
                        beam.toBreakP2 = true;
                        a2.neighborCount--;
                    }
                    else if (a1.neighborCount > 1)
                    {
                        beam.toBreakP1 = true;
                        a1.neighborCount--;
                    }
                }

                beams[beamIndex] = beam;
                anchors[point1] = a1;
                anchors[point2] = a2;
            }
        }

        public void Execute()
        {
            for (var i = 0; i < connectedComponents.Length / 2; ++i)
            {
                Execute(i);
            }
        }
    }
}