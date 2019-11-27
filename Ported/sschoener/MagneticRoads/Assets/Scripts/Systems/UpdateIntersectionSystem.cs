using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateTransformSystem))]
    public class UpdateIntersectionSystem : JobComponentSystem
    {
        RoadQueueSystem m_RoadQueueSystem;
        EndSimulationEntityCommandBufferSystem m_EndSimulationEntityCommandBufferSystem;

        struct ChangeQueueEvent
        {
            public QueueEntry QueueEntry;
            public SplinePosition From;
            public SplinePosition To;

            public ushort Intersection;
            public sbyte IntersectionSide;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            m_RoadQueueSystem = World.GetExistingSystem<RoadQueueSystem>();
            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            int frame = 1 + UnityEngine.Time.frameCount;
            var roadSetup = GetSingleton<RoadSetupComponent>();
            var splineBlob = roadSetup.Splines;
            var intersectionBlob = roadSetup.Intersections;
            var occupation = m_RoadQueueSystem.IntersectionOccupation;

            var changeQueue = new NativeQueue<ChangeQueueEvent>(Allocator.TempJob);
            var changeQueueParallel = changeQueue.AsParallelWriter();

            var queues = m_RoadQueueSystem.Queues;
            var queueEntries = m_RoadQueueSystem.QueueEntries;
            Entities.ForEach((Entity entity, ref LocalIntersectionComponent localIntersection, ref OnSplineComponent onSpline, ref CarSpeedComponent speed, ref VehicleStateComponent vehicleState, in CoordinateSystemComponent coords) =>
            {
                if (vehicleState == VehicleState.OnIntersection)
                {
                    if (speed.SplineTimer < 1)
                        return;

                    // we're exiting an intersection - make sure the next road
                    // segment has room for us before we proceed
                    var queueIndex = RoadQueueSystem.GetQueueIndex(onSpline.Value);
                    if (queues[queueIndex].Length < splineBlob.Value.Splines[onSpline.Value.Spline].MaxCarCount)
                    {
                        ref var fromSpline = ref splineBlob.Value.Splines[localIntersection.FromSpline.Spline];
                        ushort intersection = localIntersection.FromSpline.Direction > 0 ? fromSpline.EndIntersection : fromSpline.StartIntersection;
                        onSpline.Value.Dirty = true;
                        speed.SplineTimer = 0f;
                        changeQueueParallel.Enqueue(
                            new ChangeQueueEvent
                            {
                                QueueEntry = new QueueEntry
                                {
                                    Entity = entity,
                                    SplineTimer = 0
                                },
                                From = SplinePosition.Invalid,
                                To = onSpline.Value,
                                Intersection = intersection,
                                IntersectionSide = localIntersection.Side
                            });
                    }
                    else
                    {
                        speed.SplineTimer = 1f;
                        speed.NormalizedSpeed = 0f;
                    }
                }
                else if (vehicleState == VehicleState.EnteringIntersection)
                {
                    ref var fromSpline = ref splineBlob.Value.Splines[localIntersection.FromSpline.Spline];
                    ushort intersection = localIntersection.FromSpline.Direction > 0 ? fromSpline.EndIntersection : fromSpline.StartIntersection;
                    changeQueueParallel.Enqueue(
                        new ChangeQueueEvent
                        {
                            QueueEntry = new QueueEntry
                            {
                                Entity = entity,
                                SplineTimer = speed.SplineTimer
                            },
                            From = localIntersection.FromSpline,
                            To = SplinePosition.Invalid,
                            Intersection = intersection,
                            IntersectionSide = localIntersection.Side
                        });
                }
                else if (vehicleState == VehicleState.OnRoad)
                {
                    if (speed.SplineTimer < 1)
                    {
                        var queueIndex = RoadQueueSystem.GetQueueIndex(onSpline.Value);
                        var queue = queues[queueIndex];
                        for (int i = queue.Begin; i < queue.End; i++)
                        {
                            if (queueEntries[i].Entity == entity)
                            {
                                var queueEntry = queueEntries[i];
                                queueEntry.SplineTimer = speed.SplineTimer;
                                queueEntries[i] = queueEntry;
                                break;
                            }
                        }

                        return;
                    }

                    // we're exiting a road segment - first, we need to know
                    // which intersection we're entering
                    ushort intersection;
                    {
                        ref var blobSpline = ref splineBlob.Value.Splines[onSpline.Value.Spline];
                        if (onSpline.Value.Direction == 1)
                        {
                            intersection = blobSpline.EndIntersection;
                            localIntersection.Bezier.start = blobSpline.Bezier.end;
                        }
                        else
                        {
                            intersection = blobSpline.StartIntersection;
                            localIntersection.Bezier.start = blobSpline.Bezier.start;
                        }
                    }

                    // make sure that our side of the intersection (top/bottom)
                    // is empty before we enter
                    if (occupation[intersection / 4][(uint)(intersection % 4 + (localIntersection.Side + 1) / 2)])
                    {
                        speed.SplineTimer = 1f;
                        speed.NormalizedSpeed = 0f;
                    }
                    else
                    {
                        // now we need to know which road segment we'll move into
                        // (dead-ends force u-turns, but otherwise, u-turns are not allowed)
                        int newSplineIndex = 0;
                        ref var intersectionNeighbors = ref intersectionBlob.Value.Intersections[intersection].Neighbors;
                        if (intersectionNeighbors.Count > 1)
                        {
                            var rng = new Random((uint)((entity.Index + 1) * frame * 47701));
                            int mySplineIndex = intersectionNeighbors.IndexOfSpline(onSpline.Value.Spline);
                            newSplineIndex = rng.NextInt(intersectionNeighbors.Count - 1);
                            if (newSplineIndex >= mySplineIndex)
                            {
                                newSplineIndex++;
                            }
                        }

                        ushort newSpline;
                        unsafe
                        {
                            newSpline = intersectionNeighbors.Splines[newSplineIndex];
                        }

                        var previousSpline = onSpline.Value;

                        // to avoid flipping between top/bottom of our roads,
                        // we need to know our new spline's normal at our entrance point
                        float3 newNormal;
                        ref var newBlobSpline = ref splineBlob.Value.Splines[newSpline];
                        if (newBlobSpline.StartIntersection == intersection)
                        {
                            onSpline.Value.Direction = 1;
                            newNormal = newBlobSpline.Geometry.startNormal;
                            localIntersection.Bezier.end = newBlobSpline.Bezier.start;
                        }
                        else
                        {
                            onSpline.Value.Direction = -1;
                            newNormal = newBlobSpline.Geometry.endNormal;
                            localIntersection.Bezier.end = newBlobSpline.Bezier.end;
                        }

                        // now we'll prepare our intersection spline - this lets us
                        // create a "temporary lane" inside the current intersection
                        {
                            ref var intersectionData = ref intersectionBlob.Value.Intersections[intersection];
                            var pos = intersectionData.Position;
                            var norm = intersectionData.Normal;
                            localIntersection.Bezier.anchor1 = (pos + localIntersection.Bezier.start) * .5f;
                            localIntersection.Bezier.anchor2 = (pos + localIntersection.Bezier.end) * .5f;
                            localIntersection.Geometry.startTangent = math.round(math.normalize(pos - localIntersection.Bezier.start));
                            localIntersection.Geometry.endTangent = math.round(math.normalize(pos - localIntersection.Bezier.end));
                            localIntersection.Geometry.startNormal = norm;
                            localIntersection.Geometry.endNormal = norm;
                        }

                        if (onSpline.Value.Spline == newSpline)
                        {
                            // u-turn - make our intersection spline more rounded than usual
                            float3 perp = math.cross(localIntersection.Geometry.startTangent, localIntersection.Geometry.startNormal);
                            localIntersection.Bezier.anchor1 += .5f * roadSetup.IntersectionSize * localIntersection.Geometry.startTangent;
                            localIntersection.Bezier.anchor2 += .5f * roadSetup.IntersectionSize * localIntersection.Geometry.startTangent;
                            localIntersection.Bezier.anchor1 -= localIntersection.Side * roadSetup.TrackRadius * .5f * perp;
                            localIntersection.Bezier.anchor2 += localIntersection.Side * roadSetup.TrackRadius * .5f * perp;
                        }

                        localIntersection.FromSpline = previousSpline;
                        localIntersection.Length = localIntersection.Bezier.MeasureLength(roadSetup.SplineResolution);

                        // should we be on top of or underneath the intersection?
                        localIntersection.Side = (sbyte)(math.dot(localIntersection.Geometry.startNormal, coords.Up) > 0f ? 1 : -1);

                        vehicleState.Value = VehicleState.EnteringIntersection;
                        onSpline.Value.Dirty = true;

                        // to maintain our current orientation, should we be
                        // on top of or underneath our next road segment?
                        // (each road segment has its own "up" direction, at each end)
                        onSpline.Value.Side = (sbyte)(math.dot(newNormal, coords.Up) > 0f ? 1 : -1);

                        // add "leftover" spline timer value to our new spline timer
                        // (avoids a stutter when changing between splines)
                        speed.SplineTimer = (speed.SplineTimer - 1f) * splineBlob.Value.Splines[onSpline.Value.Spline].MeasuredLength / localIntersection.Length;
                        onSpline.Value.Spline = newSpline;
                    }
                }
            }).WithName("UpdateIntersection").Schedule(inputDeps).Complete();
            
            var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            Job.WithCode(() =>
            {
                int c = changeQueue.Count;
                for (int i = 0; i < c; i++)
                {
                    var changeEvent = changeQueue.Dequeue();
                    if (changeEvent.To.Spline != UInt16.MaxValue)
                    {
                        var toIdx = RoadQueueSystem.GetQueueIndex(changeEvent.To);
                        var toQueue = queues[toIdx];
                        queueEntries[toQueue.End] = changeEvent.QueueEntry;
                        toQueue.Length += 1;
                        queues[toIdx] = toQueue;

                        int intersection = changeEvent.Intersection;
                        int side = changeEvent.IntersectionSide;
                        var occupied = occupation[intersection / 4];
                        occupied[(uint)(intersection % 4 + (side + 1) / 2)] = false;
                        occupation[intersection / 4] = occupied;
                        ecb.SetComponent(changeEvent.QueueEntry.Entity, new VehicleStateComponent
                        {
                            Value = VehicleState.OnRoad
                        });
                    }

                    if (changeEvent.From.Spline != UInt16.MaxValue)
                    {
                        int intersection = changeEvent.Intersection;
                        int side = changeEvent.IntersectionSide;
                        var occupied = occupation[intersection / 4];
                        uint occupationIndex = (uint)(intersection % 4 + (side + 1) / 2);
                        if (occupied[occupationIndex])
                            continue;
                        occupied[occupationIndex] = true;
                        occupation[intersection / 4] = occupied;

                        var fromIdx = RoadQueueSystem.GetQueueIndex(changeEvent.From);
                        var fromQueue = queues[fromIdx];

                        Debug.Assert(queueEntries[fromQueue.Begin].Entity == changeEvent.QueueEntry.Entity);
                        for (int q = fromQueue.Begin; q < fromQueue.End - 1; q++)
                            queueEntries[q] = queueEntries[q + 1];
                        fromQueue.Length -= 1;
                        queues[fromIdx] = fromQueue;

                        ecb.SetComponent(changeEvent.QueueEntry.Entity, new VehicleStateComponent
                        {
                            Value = VehicleState.OnIntersection
                        });
                    }
                }
            }).Run();
            changeQueue.Dispose();
            return default;
        }
    }
}
