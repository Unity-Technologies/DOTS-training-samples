using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateTransformSystem))]
    public class UpdateIntersectionSystem : JobComponentSystem
    {
        struct ChangeQueueEvent
        {
            public QueueEntry QueueEntry;
            public SplinePosition From;
            public SplinePosition To;
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            int frame = 1 + UnityEngine.Time.frameCount;
            var roads = GetSingleton<RoadSetupComponent>();
            var splineBlob = roads.Splines;
            var intersectionBlob = roads.Intersections;
            var occupation = Intersections.Occupied;
            
            var changeQueue = new NativeQueue<ChangeQueueEvent>(Allocator.TempJob);
            var changeQueueParallel = changeQueue.AsParallelWriter();
            
            var roadSetup = GetSingleton<RoadSetupComponent>();
            Entities.ForEach((Entity entity, ref LocalIntersectionComponent localIntersection, ref OnSplineComponent onSpline, ref CarSpeedComponent speed, in CoordinateSystemComponent coords) =>
            {
                if (onSpline.Value.InIntersection)
                {
                    if (speed.SplineTimer < 1)
                        return;
                    // we're exiting an intersection - make sure the next road
                    // segment has room for us before we proceed
                    if (TrackSplines.GetQueue(onSpline.Value).Count <= splineBlob.Value.Splines[onSpline.Value.Spline].MaxCarCount)
                    {
                        var occupied = occupation[localIntersection.Intersection];
                        occupied[(localIntersection.Side + 1) / 2] = false;
                        occupation[localIntersection.Intersection] = occupied;
                        onSpline.Value.InIntersection = false;
                        onSpline.Value.Dirty = true;
                        speed.SplineTimer = 0f;
                    }
                    else
                    {
                        speed.SplineTimer = 1f;
                        speed.NormalizedSpeed = 0f;
                    }
                }
                else
                {
                    if (speed.SplineTimer < 1)
                    {
                        var queue = TrackSplines.GetQueue(onSpline.Value);
                        for (int i = 0; i < queue.Count; i++)
                        {
                            if (queue[i].Entity == entity)
                            {
                                var queueEntry = queue[i];
                                queueEntry.SplineTimer = speed.SplineTimer;
                                queue[i] = queueEntry;
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
                    if (occupation[intersection][(localIntersection.Side + 1) / 2])
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

                        localIntersection.Intersection = intersection;
                        localIntersection.Length = localIntersection.Bezier.MeasureLength(roadSetup.SplineResolution);

                        onSpline.Value.InIntersection = true;
                        onSpline.Value.Dirty = true;

                        // to maintain our current orientation, should we be
                        // on top of or underneath our next road segment?
                        // (each road segment has its own "up" direction, at each end)
                        onSpline.Value.Side = (sbyte) (math.dot(newNormal, coords.Up) > 0f ? 1 : -1);

                        // should we be on top of or underneath the intersection?
                        localIntersection.Side = (sbyte) (math.dot(localIntersection.Geometry.startNormal, coords.Up) > 0f ? 1 : -1);

                        // block other cars from entering this intersection
                        var occupied = occupation[intersection];
                        occupied[(localIntersection.Side + 1) / 2] = true;
                        occupation[intersection] = occupied;

                        // add "leftover" spline timer value to our new spline timer
                        // (avoids a stutter when changing between splines)
                        speed.SplineTimer = (speed.SplineTimer - 1f) * splineBlob.Value.Splines[onSpline.Value.Spline].MeasuredLength / localIntersection.Length;
                        onSpline.Value.Spline = newSpline;

                        changeQueueParallel.Enqueue(
                            new ChangeQueueEvent
                            {
                                QueueEntry = new QueueEntry
                                {
                                    Entity = entity,
                                    SplineTimer = speed.SplineTimer
                                },
                                From = previousSpline,
                                To = onSpline.Value
                            });
                    }
                }
            }).WithoutBurst().WithName("UpdateIntersection").Schedule(inputDeps).Complete();

            Job.WithCode(() =>
            {
                int c = changeQueue.Count;
                for (int i = 0; i < c; i++)
                {
                    var changeEvent = changeQueue.Dequeue();
                    TrackSplines.GetQueue(changeEvent.To).Add(changeEvent.QueueEntry);
                    var fromQueue = TrackSplines.GetQueue(changeEvent.From);
                    Debug.Assert(fromQueue[0].Entity == changeEvent.QueueEntry.Entity); 
                    fromQueue.RemoveAt(0);
                }
            }).WithoutBurst().Run();
            changeQueue.Dispose();
            return default;
        }
    }
}
